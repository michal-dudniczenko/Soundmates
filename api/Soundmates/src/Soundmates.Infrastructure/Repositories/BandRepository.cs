using Microsoft.EntityFrameworkCore;
using Soundmates.Domain.Entities;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Infrastructure.Database;

namespace Soundmates.Infrastructure.Repositories;

public class BandRepository(
    ApplicationDbContext _context
) : IBandRepository
{
    public async Task<Band?> GetByUserIdAsync(Guid userId)
    {
        return await _context.Bands
            .AsNoTracking()
            .Include(b => b.Members)
            .Include(a => a.User)
                .ThenInclude(u => u.Tags)
            .Include(a => a.User)
                .ThenInclude(u => u.MusicSamples)
            .Include(a => a.User)
                .ThenInclude(u => u.ProfilePictures)
            .FirstOrDefaultAsync(a => a.UserId == userId);
    }

    public async Task<IEnumerable<Band>> GetPotentialMatchesAsync(Guid userId, int limit, int offset)
    {
        var userMatchPreference = await _context.UserMatchPreferences
            .AsNoTracking()
            .Include(ump => ump.Tags)
                .ThenInclude(t => t.TagCategory)
            .FirstOrDefaultAsync(ump => ump.UserId == userId)
            ?? throw new InvalidOperationException($"Match preference data for user: {userId} not found.");

        if (!userMatchPreference.ShowBands)
        {
            return [];
        }

        IQueryable<Band> bands = _context.Bands
            .AsNoTracking()
            .Include(b => b.Members)
            .Include(b => b.User)
                .ThenInclude(u => u.Tags)
            .Include(b => b.User)
                .ThenInclude(u => u.MusicSamples)
            .Include(b => b.User)
                .ThenInclude(u => u.ProfilePictures);

        bands = bands.Where(b => b.User.IsActive && b.User.IsEmailConfirmed && !b.User.IsFirstLogin);

        if (userMatchPreference.MaxDistance is not null)
        {
            // TODO calculate distance
        }

        if (userMatchPreference.CountryId is not null)
        {
            bands = bands.Where(b => b.User.CountryId == userMatchPreference.CountryId);
        }

        if (userMatchPreference.CityId is not null)
        {
            bands = bands.Where(b => b.User.CityId == userMatchPreference.CityId);
        }

        if (userMatchPreference.BandMinMembersCount is not null)
        {
            bands = bands.Where(b => b.Members.Count >= userMatchPreference.BandMinMembersCount);
        }

        if (userMatchPreference.BandMaxMembersCount is not null)
        {
            bands = bands.Where(b => b.Members.Count <= userMatchPreference.BandMaxMembersCount);
        }

        foreach (var tag in userMatchPreference.Tags.Where(t => t.TagCategory.IsForBand))
        {
            bands = bands.Where(b => b.User.Tags.Any(t => t.Id == tag.Id));
        }

        return await bands
            .OrderBy(a => a.Id)
            .Skip(offset)
            .Take(limit).ToListAsync();
    }

    public async Task UpdateAddAsync(Band entity, IList<Guid> tagsIds, IList<Guid> musicSamplesOrder, IList<Guid> profilePicturesOrder)
    {
        var existingUser = await _context.Users.FindAsync(entity.UserId)
            ?? throw new InvalidOperationException($"User with id {entity.UserId} was not found.");

        var bandTags = await _context.Tags
            .Include(t => t.TagCategory)
            .Where(t => t.TagCategory.IsForBand)
            .ToListAsync();

        existingUser.Tags.Clear();
        foreach (var tagId in tagsIds)
        {
            var tag = bandTags.FirstOrDefault(t => t.Id == tagId)
                ?? throw new InvalidOperationException($"Invalid tag id provided: {tagId}");

            existingUser.Tags.Add(tag);
        }

        var existingMusicSamples = await _context.MusicSamples.Where(ms => ms.UserId == existingUser.Id).ToListAsync();

        if (musicSamplesOrder.Count != musicSamplesOrder.Distinct().Count())
        {
            throw new InvalidOperationException("Provided list of music samples contained duplicates.");
        }

        existingUser.MusicSamples.Clear();
        int displayOrder = 0;
        foreach (var sampleId in musicSamplesOrder)
        {
            var sample = existingMusicSamples.FirstOrDefault(ms => ms.Id == sampleId)
                ?? throw new InvalidOperationException($"Not existing music sample provided with id: {sampleId}");

            sample.DisplayOrder = displayOrder;
            existingUser.MusicSamples.Add(sample);

            displayOrder++;
        }

        var existingProfilePictures = await _context.ProfilePictures.Where(pp => pp.UserId == existingUser.Id).ToListAsync();

        if (profilePicturesOrder.Count != profilePicturesOrder.Distinct().Count())
        {
            throw new InvalidOperationException("Provided list of profile pictures contained duplicates.");
        }

        existingUser.ProfilePictures.Clear();
        displayOrder = 0;
        foreach (var pictureId in profilePicturesOrder)
        {
            var picture = existingProfilePictures.FirstOrDefault(pp => pp.Id == pictureId)
                ?? throw new InvalidOperationException($"Not existing profile picture provided with id: {pictureId}");

            picture.DisplayOrder = displayOrder;
            existingUser.ProfilePictures.Add(picture);

            displayOrder++;
        }

        existingUser.IsBand = true;
        existingUser.IsFirstLogin = false;

        existingUser.Name = entity.User.Name;
        existingUser.Description = entity.User.Description;
        existingUser.CountryId = entity.User.CountryId;
        existingUser.CityId = entity.User.CityId;

        var existingBand = await _context.Bands
            .Where(a => a.UserId == existingUser.Id)
            .FirstOrDefaultAsync();

        if (existingBand is null)
        {
            entity.User = existingUser;

            _context.Bands.Add(entity);
        }
        else
        {
            existingBand.Members.Clear();

            foreach (var member in entity.Members)
            {
                existingBand.Members.Add(member);
            }
        }

        await _context.SaveChangesAsync();
    }
}

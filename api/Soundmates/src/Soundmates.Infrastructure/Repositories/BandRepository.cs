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
        var bandTags = await _context.Tags
            .Include(t => t.TagCategory)
            .Where(t => t.TagCategory.IsForBand)
            .ToListAsync();

        entity.User.Tags.Clear();
        foreach (var tagId in tagsIds)
        {
            var tag = bandTags.FirstOrDefault(t => t.Id == tagId)
                ?? throw new InvalidOperationException($"Invalid tag id provided: {tagId}");

            entity.User.Tags.Add(tag);
        }

        var existingMusicSamples = await _context.MusicSamples.Where(ms => ms.UserId == entity.UserId).ToListAsync();

        if (musicSamplesOrder.Count != musicSamplesOrder.Distinct().Count())
        {
            throw new InvalidOperationException("Provided list of music samples contained duplicates.");
        }

        entity.User.MusicSamples.Clear();
        int displayOrder = 0;
        foreach (var sampleId in musicSamplesOrder)
        {
            var sample = existingMusicSamples.FirstOrDefault(ms => ms.Id == sampleId)
                ?? throw new InvalidOperationException($"Not existing music sample provided with id: {sampleId}");

            sample.DisplayOrder = displayOrder;
            entity.User.MusicSamples.Add(sample);

            displayOrder++;
        }

        var existingProfilePictures = await _context.ProfilePictures.Where(pp => pp.UserId == entity.UserId).ToListAsync();

        if (profilePicturesOrder.Count != profilePicturesOrder.Distinct().Count())
        {
            throw new InvalidOperationException("Provided list of profile pictures contained duplicates.");
        }

        entity.User.ProfilePictures.Clear();
        displayOrder = 0;
        foreach (var pictureId in profilePicturesOrder)
        {
            var picture = existingProfilePictures.FirstOrDefault(pp => pp.Id == pictureId)
                ?? throw new InvalidOperationException($"Not existing profile picture provided with id: {pictureId}");

            picture.DisplayOrder = displayOrder;
            entity.User.ProfilePictures.Add(picture);

            displayOrder++;
        }

        _context.Users.Update(entity.User);

        var bandExists = await _context.Bands.AnyAsync(a => a.Id == entity.Id);

        if (bandExists)
        {
            _context.Bands.Update(entity);
        }
        else
        {
            _context.Bands.Add(entity);
        }

        await _context.SaveChangesAsync();
    }
}

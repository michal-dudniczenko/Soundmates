using Microsoft.EntityFrameworkCore;
using Soundmates.Domain.Entities;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Infrastructure.Database;

namespace Soundmates.Infrastructure.Repositories;

public class ArtistRepository(
    ApplicationDbContext _context
) : IArtistRepository
{
    public async Task<Artist?> GetByUserIdAsync(Guid userId)
    {
        return await _context.Artists
            .AsNoTracking()
            .Include(a => a.User)
                .ThenInclude(u => u.Tags)
            .Include(a => a.User)
                .ThenInclude(u => u.MusicSamples)
            .Include(a => a.User)
                .ThenInclude(u => u.ProfilePictures)
            .FirstOrDefaultAsync(a => a.UserId == userId);
    }

    public async Task<IEnumerable<Artist>> GetPotentialMatchesAsync(Guid userId, int limit, int offset)
    {
        var userMatchPreference = await _context.UserMatchPreferences
            .AsNoTracking()
            .Include(ump => ump.Tags)
                .ThenInclude(t => t.TagCategory)
            .FirstOrDefaultAsync(ump => ump.UserId == userId)
            ?? throw new InvalidOperationException($"Match preference data for user: {userId} not found.");

        if (!userMatchPreference.ShowArtists)
        {
            return [];
        }

        IQueryable<Artist> artists = _context.Artists
            .AsNoTracking()
            .Include(a => a.User)
                .ThenInclude(u => u.Tags)
            .Include(a => a.User)
                .ThenInclude(u => u.MusicSamples)
            .Include(a => a.User)
                .ThenInclude(u => u.ProfilePictures);

        artists = artists.Where(a => a.User.IsActive && a.User.IsEmailConfirmed && !a.User.IsFirstLogin);

        if (userMatchPreference.MaxDistance is not null)
        {
            // TODO calculate distance
        }

        if (userMatchPreference.CountryId is not null)
        {
            artists = artists.Where(a => a.User.CountryId == userMatchPreference.CountryId);
        }

        if (userMatchPreference.CityId is not null)
        {
            artists = artists.Where(a => a.User.CityId == userMatchPreference.CityId);
        }

        if (userMatchPreference.ArtistMinAge is not null)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            artists = artists.Where(a => today.Year - a.BirthDate.Year >= userMatchPreference.ArtistMinAge);
        }

        if (userMatchPreference.ArtistMaxAge is not null)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            artists = artists.Where(a => today.Year - a.BirthDate.Year <= userMatchPreference.ArtistMaxAge);
        }

        if (userMatchPreference.ArtistGenderId is not null)
        {
            artists = artists.Where(a => a.GenderId == userMatchPreference.ArtistGenderId);
        }

        foreach (var tag in userMatchPreference.Tags.Where(t => !t.TagCategory.IsForBand))
        {
            artists = artists.Where(a => a.User.Tags.Any(t => t.Id == tag.Id));
        }

        return await artists
            .OrderBy(a => a.Id)
            .Skip(offset)
            .Take(limit).ToListAsync();
    }

    public async Task UpdateAddAsync(Artist entity, IList<Guid> tagsIds, IList<Guid> musicSamplesOrder, IList<Guid> profilePicturesOrder)
    {
        var existingUser = await _context.Users
            .Include(u => u.Tags)
            .Include(u => u.MusicSamples)
            .Include(u => u.ProfilePictures)
            .FirstOrDefaultAsync(u => u.Id == entity.UserId)
            ?? throw new InvalidOperationException($"User with id {entity.UserId} was not found.");

        var artistTags = await _context.Tags
            .Include(t => t.TagCategory)
            .Where(t => !t.TagCategory.IsForBand)
            .ToListAsync();

        existingUser.Tags.Clear();
        foreach (var tagId in tagsIds)
        {
            var tag = artistTags.FirstOrDefault(t => t.Id == tagId)
                ?? throw new InvalidOperationException($"Invalid tag id provided: {tagId}");

            existingUser.Tags.Add(tag);
        }

        if (musicSamplesOrder.Count != musicSamplesOrder.Distinct().Count())
        {
            throw new InvalidOperationException("Provided list of music samples contained duplicates.");
        }

        var existingMusicSamples = existingUser.MusicSamples.ToList();

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

        if (profilePicturesOrder.Count != profilePicturesOrder.Distinct().Count())
        {
            throw new InvalidOperationException("Provided list of profile pictures contained duplicates.");
        }

        var existingProfilePictures = existingUser.ProfilePictures.ToList();

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

        existingUser.IsBand = false;
        existingUser.IsFirstLogin = false;

        existingUser.Name = entity.User.Name;
        existingUser.Description = entity.User.Description;
        existingUser.CountryId = entity.User.CountryId;
        existingUser.CityId = entity.User.CityId;

        var existingArtist = await _context.Artists
            .Where(a => a.UserId == existingUser.Id)
            .FirstOrDefaultAsync();

        if (existingArtist is null)
        {
            entity.User = existingUser;

            _context.Add(entity);
        }
        else
        {
            existingArtist.BirthDate = entity.BirthDate;
            existingArtist.GenderId = entity.GenderId;
        }

        await _context.SaveChangesAsync();
    }
}

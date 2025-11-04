using Microsoft.EntityFrameworkCore;
using Soundmates.Domain.Entities;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Infrastructure.Database;

namespace Soundmates.Infrastructure.Repositories;

public class ArtistRepository(
    ApplicationDbContext _context
) : IArtistRepository
{
    private record ScoredArtist(Artist Artist, double MatchScore);
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
            return Enumerable.Empty<Artist>();
        }

        IQueryable<Artist> artists = _context.Artists
            .AsNoTracking()
            .Include(a => a.User)
                .ThenInclude(u => u.Tags)
            .Include(a => a.User)
                .ThenInclude(u => u.MusicSamples)
            .Include(a => a.User)
                .ThenInclude(u => u.ProfilePictures);

        artists = artists.Where(a => a.User.IsActive && a.User.IsEmailConfirmed && !a.User.IsFirstLogin && a.UserId != userId);
        artists = artists.Where(a =>
             !_context.Likes.Any(l => l.GiverId == userId && l.ReceiverId == a.UserId)
             && !_context.Dislikes.Any(dl => dl.GiverId == userId && dl.ReceiverId == a.UserId));

        if (userMatchPreference.MaxDistance is not null)
        {
            var originCity = userMatchPreference.User?.City;
            if (originCity is not null)
            {
                artists = artists.Where(a => a.User.City != null);
                artists = artists.Where(a =>
                    CalculateHaversineDistance(originCity.Latitude, originCity.Longitude, a.User.City!.Latitude, a.User.City!.Longitude
                    ) <= userMatchPreference.MaxDistance.Value
                );
            }
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
            var minAgeCutoffDate = today.AddYears(-userMatchPreference.ArtistMinAge.Value);
            artists = artists.Where(a => a.BirthDate <= minAgeCutoffDate);
        }

        if (userMatchPreference.ArtistMaxAge is not null)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var maxAgeCutoffDate = today.AddYears(-(userMatchPreference.ArtistMaxAge.Value + 1));
            artists = artists.Where(a => a.BirthDate > maxAgeCutoffDate);
        }

        if (userMatchPreference.ArtistGenderId is not null)
        {
            artists = artists.Where(a => a.GenderId == userMatchPreference.ArtistGenderId);
        }

        foreach (var tag in userMatchPreference.Tags.Where(t => !t.TagCategory.IsForBand))
        {
            artists = artists.Where(a => a.User.Tags.Any(t => t.Id == tag.Id));
        }

        var scoredQuery = ScorePotentialMatches(artists, userMatchPreference);

        return await scoredQuery
            .OrderByDescending(x => x.MatchScore)
            .Skip(offset)
            .Take(limit)
            .Select(x => x.Artist)
            .ToListAsync();
    }

    public async Task UpdateAddAsync(Artist entity, IList<Guid> tagsIds, IList<Guid> musicSamplesOrder, IList<Guid> profilePicturesOrder)
    {
        var existingUser = await _context.Users.FindAsync(entity.UserId)
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
    private static double CalculateHaversineDistance(double originLat, double originLon, double destLat, double destLon)
    {
        const double earthRadiusKm = 6371.0;
        double originLatRad = originLat * (Math.PI / 180.0);
        double originLonRad = originLon * (Math.PI / 180.0);
        double destLatRad = destLat * (Math.PI / 180.0);
        double destLonRad = destLon * (Math.PI / 180.0);

        // Haversine formula
        double dLat = (destLatRad - originLatRad) / 2.0;
        double dLon = (destLonRad - originLonRad) / 2.0;
        double a = Math.Pow(Math.Sin(dLat), 2.0) +
                   Math.Cos(originLatRad) * Math.Cos(destLatRad) *
                   Math.Pow(Math.Sin(dLon), 2.0);

        double c = 2.0 * Math.Asin(Math.Sqrt(a));
        return earthRadiusKm * c;
    }

    private static IQueryable<ScoredArtist> ScorePotentialMatches(IQueryable<Artist> artists, UserMatchPreference userMatchPreference)
    {
        var userPreferenceTagIds = userMatchPreference.Tags
            .Where(t => !t.TagCategory.IsForBand)
            .Select(t => t.Id)
            .ToList();

        var originCity = userMatchPreference.User?.City;
        var maxDistance = userMatchPreference.MaxDistance;

        return artists.Select(a => new ScoredArtist(
            a,
            (a.User.Tags.Count(t => userPreferenceTagIds.Contains(t.Id)) * 100.0) +
            (originCity == null || a.User.City == null || maxDistance == null || maxDistance.Value == 0 ? 0.0 :
             (1.0 - (CalculateHaversineDistance(originCity.Latitude, originCity.Longitude, a.User.City.Latitude, a.User.City.Longitude) / maxDistance.Value)) * 100.0)
        ));
    }
}

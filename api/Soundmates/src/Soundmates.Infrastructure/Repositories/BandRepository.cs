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

        bands = bands.Where(b => b.Id != userId && b.User.IsActive && b.User.IsEmailConfirmed && !b.User.IsFirstLogin && b.UserId != userId);
        bands = bands.Where(b =>
             !_context.Likes.Any(l => l.GiverId == userId && l.ReceiverId == b.UserId)
             && !_context.Dislikes.Any(dl => dl.GiverId == userId && dl.ReceiverId == b.UserId));

        if (userMatchPreference.MaxDistance is not null)
        {
            var originCity = userMatchPreference.User?.City;
            if (originCity is not null)
            {
                bands = bands.Where(a => a.User.City != null);
                bands = bands.Where(a =>
                    CalculateHaversineDistance(originCity.Latitude, originCity.Longitude, a.User.City!.Latitude, a.User.City!.Longitude
                    ) <= userMatchPreference.MaxDistance.Value
                );
            }
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
        var existingUser = await _context.Users
            .Include(u => u.Tags)
            .Include(u => u.MusicSamples)
            .Include(u => u.ProfilePictures)
            .FirstOrDefaultAsync(u => u.Id == entity.UserId)
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

        existingUser.IsBand = true;
        existingUser.IsFirstLogin = false;

        existingUser.Name = entity.User.Name;
        existingUser.Description = entity.User.Description;
        existingUser.CountryId = entity.User.CountryId;
        existingUser.CityId = entity.User.CityId;

        var existingBand = await _context.Bands
            .Include(b => b.Members)
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
}

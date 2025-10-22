using Microsoft.EntityFrameworkCore;
using Soundmates.Domain.Entities;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Infrastructure.Database;

namespace Soundmates.Infrastructure.Repositories;

public class MatchPreferenceRepository(
    ApplicationDbContext _context
) : IMatchPreferenceRepository
{
    public async Task AddUpdateAsync(UserMatchPreference entity, IList<Guid>? filterTagsIds = null)
    {
        List<Tag> tags = [];
        if (filterTagsIds is not null && filterTagsIds.Count != 0)
        {
            tags = await _context.Tags
                .Where(t => filterTagsIds.Contains(t.Id))
                .ToListAsync();

            foreach (var tagId in filterTagsIds)
            {
                if (!tags.Any(t => t.Id == tagId))
                {
                    throw new InvalidOperationException($"Invalid tag id provided: {tagId}");
                }
            }
        }

        var existing = await _context.UserMatchPreferences
            .Include(mp => mp.Tags)
            .FirstOrDefaultAsync(mp => mp.UserId == entity.UserId);

        if (existing is null)
        {
            foreach (var tag in tags)
            {
                entity.Tags.Add(tag);
            }

            _context.UserMatchPreferences.Add(entity);
        }
        else
        {
            existing.ShowArtists = entity.ShowArtists;
            existing.ShowBands = entity.ShowBands;
            existing.MaxDistance = entity.MaxDistance;
            existing.CountryId = entity.CountryId;
            existing.CityId = entity.CityId;
            existing.ArtistMinAge = entity.ArtistMinAge;
            existing.ArtistMaxAge = entity.ArtistMaxAge;
            existing.ArtistGenderId = entity.ArtistGenderId;
            existing.BandMinMembersCount = entity.BandMinMembersCount;
            existing.BandMaxMembersCount = entity.BandMaxMembersCount;
            existing.UserId = entity.UserId;

            existing.Tags.Clear();

            foreach(var tag in tags)
            {
                existing.Tags.Add(tag);
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task<UserMatchPreference?> GetUserMatchPreferenceAsync(Guid userId)
    {
        return await _context.UserMatchPreferences
            .AsNoTracking()
            .Include(mp => mp.Tags)
            .FirstOrDefaultAsync(mp => mp.UserId == userId);
    }
}

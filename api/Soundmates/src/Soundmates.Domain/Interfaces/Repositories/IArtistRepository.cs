using Soundmates.Domain.Entities;

namespace Soundmates.Domain.Interfaces.Repositories;

public interface IArtistRepository
{
    Task<Artist?> GetByUserIdAsync(Guid entityId);
    Task<Artist> AddAsync(Artist entity);
    Task UpdateAsync(Artist entity);
    Task<IEnumerable<Artist>> GetPotentialMatchesAsync(Guid entityId, int limit, int offset);
    Task<bool> CheckIfExistsAsync(Guid entityId);
}

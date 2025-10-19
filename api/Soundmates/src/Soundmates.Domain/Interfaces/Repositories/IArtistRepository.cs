using Soundmates.Domain.Entities;

namespace Soundmates.Domain.Interfaces.Repositories;

public interface IArtistRepository
{
    Task<Artist?> GetByIdAsync(Guid entityId);
    Task<Artist> AddAsync(Artist entity);
    Task UpdateAsync(Artist entity);
    Task<IEnumerable<Artist>> GetPotentialMatches(Guid entityId);
    Task<bool> CheckIfExistsAsync(Guid entityId);
}

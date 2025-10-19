using Soundmates.Domain.Entities;

namespace Soundmates.Domain.Interfaces.Repositories;

public interface IBandRepository
{
    Task<Band?> GetByUserIdAsync(Guid entityId);
    Task<Band> AddAsync(Band entity);
    Task UpdateAsync(Band entity);
    Task<IEnumerable<Band>> GetPotentialMatchesAsync(Guid entityId, int limit, int offset);
    Task<bool> CheckIfExistsAsync(Guid entityId);
}

using Soundmates.Domain.Entities;

namespace Soundmates.Domain.Interfaces.Repositories;

public interface IBandRepository
{
    Task<Band?> GetByIdAsync(Guid entityId);
    Task<Band> AddAsync(Band entity);
    Task UpdateAsync(Band entity);
    Task<IEnumerable<Band>> GetPotentialMatches(Guid entityId);
    Task<bool> CheckIfExistsAsync(Guid entityId);
}

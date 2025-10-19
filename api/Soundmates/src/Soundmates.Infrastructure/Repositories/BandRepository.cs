using Soundmates.Domain.Entities;
using Soundmates.Domain.Interfaces.Repositories;

namespace Soundmates.Infrastructure.Repositories;

public class BandRepository : IBandRepository
{
    public Task<Band> AddAsync(Band entity)
    {
        throw new NotImplementedException();
    }

    public Task<bool> CheckIfExistsAsync(Guid entityId)
    {
        throw new NotImplementedException();
    }

    public Task<Band?> GetByUserIdAsync(Guid entityId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Band>> GetPotentialMatchesAsync(Guid entityId, int limit, int offset)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Band entity)
    {
        throw new NotImplementedException();
    }
}

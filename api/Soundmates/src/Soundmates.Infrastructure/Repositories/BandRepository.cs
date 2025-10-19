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

    public Task<Band?> GetByIdAsync(Guid entityId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Band>> GetPotentialMatches(Guid entityId)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Band entity)
    {
        throw new NotImplementedException();
    }
}

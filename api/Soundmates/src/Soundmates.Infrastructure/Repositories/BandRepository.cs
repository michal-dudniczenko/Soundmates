using Soundmates.Domain.Entities;
using Soundmates.Domain.Interfaces.Repositories;

namespace Soundmates.Infrastructure.Repositories;

public class BandRepository : IBandRepository
{
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

    public Task UpdateAddAsync(Band entity, IList<Guid> MusicSamplesOrder, IList<Guid> ProfilePicturesOrder)
    {
        throw new NotImplementedException();
    }
}

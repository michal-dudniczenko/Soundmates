using Soundmates.Domain.Entities;

namespace Soundmates.Domain.Interfaces.Repositories;

public interface IBandRepository
{
    Task<Band?> GetByUserIdAsync(Guid entityId);
    Task UpdateAddAsync(Band entity, IList<Guid> MusicSamplesOrder, IList<Guid> ProfilePicturesOrder);
    Task<IEnumerable<Band>> GetPotentialMatchesAsync(Guid entityId, int limit, int offset);
    Task<bool> CheckIfExistsAsync(Guid entityId);
}

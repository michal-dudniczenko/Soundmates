using Soundmates.Domain.Entities;

namespace Soundmates.Domain.Interfaces.Repositories;

public interface IBandRepository
{
    Task<Band?> GetByUserIdAsync(Guid userId);
    Task UpdateAddAsync(Band entity, IList<Guid> tagsIds, IList<Guid> musicSamplesOrder, IList<Guid> profilePicturesOrder);
    Task<IEnumerable<Band>> GetPotentialMatchesAsync(Guid userId, int limit, int offset);
}

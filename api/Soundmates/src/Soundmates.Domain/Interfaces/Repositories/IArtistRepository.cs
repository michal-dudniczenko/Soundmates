using Soundmates.Domain.Entities;

namespace Soundmates.Domain.Interfaces.Repositories;

public interface IArtistRepository
{
    Task<Artist?> GetByUserIdAsync(Guid userId);
    Task UpdateAddAsync(Artist entity, IList<Guid> tagsIds, IList<Guid> musicSamplesOrder, IList<Guid> profilePicturesOrder);
    Task<IEnumerable<Artist>> GetPotentialMatchesAsync(Guid userId, int limit, int offset);
}

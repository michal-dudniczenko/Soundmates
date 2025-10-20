using Soundmates.Domain.Entities;

namespace Soundmates.Domain.Interfaces.Repositories;

public interface IArtistRepository
{
    Task<Artist?> GetByUserIdAsync(Guid entityId);
    Task UpdateAddAsync(Artist entity, IList<Guid> MusicSamplesOrder, IList<Guid> ProfilePicturesOrder);
    Task<IEnumerable<Artist>> GetPotentialMatchesAsync(Guid entityId, int limit, int offset);
    Task<bool> CheckIfExistsAsync(Guid entityId);
}

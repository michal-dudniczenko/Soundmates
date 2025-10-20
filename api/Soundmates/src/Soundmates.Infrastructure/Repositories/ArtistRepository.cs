using Soundmates.Domain.Entities;
using Soundmates.Domain.Interfaces.Repositories;

namespace Soundmates.Infrastructure.Repositories;

public class ArtistRepository : IArtistRepository
{
    public Task<bool> CheckIfExistsAsync(Guid entityId)
    {
        throw new NotImplementedException();
    }

    public Task<Artist?> GetByUserIdAsync(Guid entityId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Artist>> GetPotentialMatchesAsync(Guid entityId, int limit, int offset)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAddAsync(Artist entity, IList<Guid> MusicSamplesOrder, IList<Guid> ProfilePicturesOrder)
    {
        throw new NotImplementedException();
    }
}

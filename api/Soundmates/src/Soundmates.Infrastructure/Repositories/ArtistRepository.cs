using Soundmates.Domain.Entities;
using Soundmates.Domain.Interfaces.Repositories;

namespace Soundmates.Infrastructure.Repositories;

public class ArtistRepository : IArtistRepository
{
    public Task<Artist> AddAsync(Artist entity)
    {
        throw new NotImplementedException();
    }

    public Task<bool> CheckIfExistsAsync(Guid entityId)
    {
        throw new NotImplementedException();
    }

    public Task<Artist?> GetByIdAsync(Guid entityId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Artist>> GetPotentialMatches(Guid entityId)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Artist entity)
    {
        throw new NotImplementedException();
    }
}

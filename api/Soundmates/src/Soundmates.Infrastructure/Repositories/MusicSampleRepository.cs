using Soundmates.Domain.Entities;
using Soundmates.Domain.Interfaces.Repositories;

namespace Soundmates.Infrastructure.Repositories;

public class MusicSampleRepository : IMusicSampleRepository
{
    public Task AddAsync(MusicSample entity)
    {
        throw new NotImplementedException();
    }

    public Task RemoveAsync(Guid entityId)
    {
        throw new NotImplementedException();
    }
}

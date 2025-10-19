using Soundmates.Domain.Entities;

namespace Soundmates.Domain.Interfaces.Repositories;

public interface IMusicSampleRepository
{
    Task AddAsync(MusicSample entity);
    Task RemoveAsync(Guid entityId);
}

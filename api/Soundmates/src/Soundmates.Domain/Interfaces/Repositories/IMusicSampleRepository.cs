using Soundmates.Domain.Entities;

namespace Soundmates.Domain.Interfaces.Repositories;

public interface IMusicSampleRepository
{
    Task<MusicSample?> GetByIdAsync(Guid entityId);
    Task AddAsync(MusicSample entity);
    Task RemoveAsync(Guid entityId);
}

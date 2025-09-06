using Soundmates.Domain.Entities;

namespace Soundmates.Domain.Interfaces.Repositories;

public interface IMusicSampleRepository : IBaseRepository<MusicSample>
{
    Task<IEnumerable<MusicSample>> GetUserMusicSamplesAsync(Guid userId, int limit, int offset);
    Task<int> GetUserMusicSamplesCountAsync(Guid userId);
    Task<bool> MoveDisplayOrderUpAsync(Guid musicSampleId);
    Task<bool> MoveDisplayOrderDownAsync(Guid musicSampleId);
}

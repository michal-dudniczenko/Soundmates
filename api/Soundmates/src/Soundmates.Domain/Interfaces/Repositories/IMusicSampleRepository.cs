using Soundmates.Domain.Entities;

namespace Soundmates.Domain.Interfaces.Repositories;

public interface IMusicSampleRepository : IBaseRepository<MusicSample>
{
    Task<IEnumerable<MusicSample>> GetUserMusicSamplesAsync(int userId, int limit, int offset);
    Task<int> GetUserMusicSamplesCountAsync(int userId);
    Task<bool> MoveDisplayOrderUpAsync(int musicSampleId);
    Task<bool> MoveDisplayOrderDownAsync(int musicSampleId);
}

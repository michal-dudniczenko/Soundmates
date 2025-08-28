using Soundmates.Domain.Entities;

namespace Soundmates.Domain.Interfaces.Repositories;

public interface IMusicSampleRepository : IBaseRepository<MusicSample>
{
    Task<IEnumerable<MusicSample>> GetUserMusicSamplesAsync(int userId, int limit = 50, int offset = 0);
    Task<int> GetUserMusicSamplesCountAsync(int userId);
    Task MoveDisplayOrderUpAsync(int musicSampleId);
    Task MoveDisplayOrderDownAsync(int musicSampleId);
}

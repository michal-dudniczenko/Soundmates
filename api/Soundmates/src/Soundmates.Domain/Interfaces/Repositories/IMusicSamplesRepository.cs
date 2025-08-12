using Soundmates.Domain.Entities;

namespace Soundmates.Domain.Interfaces.Repositories;

public interface IMusicSamplesRepository : IBaseRepository<MusicSample>
{
    Task<IEnumerable<MusicSample>> GetUserMusicSamplesAsync(int userId, int limit = 50, int offset = 0);
}

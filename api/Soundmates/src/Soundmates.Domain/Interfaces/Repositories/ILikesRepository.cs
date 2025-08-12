using Soundmates.Domain.Entities;

namespace Soundmates.Domain.Interfaces.Repositories;

public interface ILikesRepository : IBaseRepository<Like>
{
    Task<IEnumerable<Like>> GetUserGivenLikesAsync(int userId, int limit = 50, int offset = 0);
    Task<IEnumerable<Like>> GetUserReceivedLikesAsync(int userId, int limit = 50, int offset = 0);
}

using Soundmates.Domain.Entities;

namespace Soundmates.Domain.Interfaces.Repositories;

public interface ILikeRepository : IBaseRepository<Like>
{
    Task<IEnumerable<Like>> GetUserGivenLikesAsync(int userId, int limit, int offset);
    Task<IEnumerable<Like>> GetUserReceivedLikesAsync(int userId, int limit, int offset);
    Task<bool> CheckIfExistsAsync(int giverId, int receiverId);
}

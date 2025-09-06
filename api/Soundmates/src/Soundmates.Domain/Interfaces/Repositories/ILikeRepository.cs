using Soundmates.Domain.Entities;

namespace Soundmates.Domain.Interfaces.Repositories;

public interface ILikeRepository : IBaseRepository<Like>
{
    Task<IEnumerable<Like>> GetUserGivenLikesAsync(Guid userId, int limit, int offset);
    Task<IEnumerable<Like>> GetUserReceivedLikesAsync(Guid userId, int limit, int offset);
    Task<bool> CheckIfExistsAsync(Guid giverId, Guid receiverId);
}

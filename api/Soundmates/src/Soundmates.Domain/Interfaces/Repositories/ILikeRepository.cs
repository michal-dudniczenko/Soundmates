using Soundmates.Domain.Entities;

namespace Soundmates.Domain.Interfaces.Repositories;

public interface ILikeRepository
{
    Task<IEnumerable<Like>> GetUserGivenLikesAsync(Guid userId, int limit, int offset);
    Task<IEnumerable<Like>> GetUserReceivedLikesAsync(Guid userId, int limit, int offset);
    Task AddAsync(Like entity);
    Task<bool> CheckIfExistsAsync(Guid giverId, Guid receiverId);
}

using Soundmates.Domain.Entities;

namespace Soundmates.Domain.Interfaces.Repositories;

public interface IMatchRepository
{
    Task AddLikeAsync(Like entity);
    Task AddDislikeAsync(Dislike entity);
    Task<IEnumerable<Match>> GetUserMatchesAsync(Guid userId, int limit, int offset);
    Task AddAsync(Match entity);
    Task<bool> CheckIfMatchExistsAsync(Guid user1Id, Guid user2Id);
    Task<bool> CheckIfLikeExistsAsync(Guid giverId, Guid receiverId);
    Task<bool> CheckIfReactionExistsAsync(Guid giverId, Guid receiverId);
}

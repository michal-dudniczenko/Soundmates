using Soundmates.Domain.Entities;

namespace Soundmates.Domain.Interfaces.Repositories;

public interface IDislikeRepository
{
    Task<IEnumerable<Dislike>> GetUserGivenDislikesAsync(Guid userId, int limit, int offset);
    Task<IEnumerable<Dislike>> GetUserReceivedDislikesAsync(Guid userId, int limit, int offset);
    Task AddAsync(Dislike entity);
    Task<bool> CheckIfExistsAsync(Guid giverId, Guid receiverId);
}

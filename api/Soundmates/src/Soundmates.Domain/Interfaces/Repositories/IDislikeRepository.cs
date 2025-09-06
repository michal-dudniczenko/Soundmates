using Soundmates.Domain.Entities;

namespace Soundmates.Domain.Interfaces.Repositories;

public interface IDislikeRepository : IBaseRepository<Dislike>
{
    Task<IEnumerable<Dislike>> GetUserGivenDislikesAsync(Guid userId, int limit, int offset);
    Task<IEnumerable<Dislike>> GetUserReceivedDislikesAsync(Guid userId, int limit, int offset);
    Task<bool> CheckIfExistsAsync(Guid giverId, Guid receiverId);
}

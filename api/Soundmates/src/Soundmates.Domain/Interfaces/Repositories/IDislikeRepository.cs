using Soundmates.Domain.Entities;

namespace Soundmates.Domain.Interfaces.Repositories;

public interface IDislikeRepository : IBaseRepository<Dislike>
{
    Task<IEnumerable<Dislike>> GetUserGivenDislikesAsync(int userId, int limit, int offset);
    Task<IEnumerable<Dislike>> GetUserReceivedDislikesAsync(int userId, int limit, int offset);
    Task<bool> CheckIfExistsAsync(int giverId, int receiverId);
}

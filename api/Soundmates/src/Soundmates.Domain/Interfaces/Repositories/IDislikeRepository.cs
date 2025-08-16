using Soundmates.Domain.Entities;

namespace Soundmates.Domain.Interfaces.Repositories;

public interface IDislikeRepository : IBaseRepository<Dislike>
{
    Task<IEnumerable<Dislike>> GetUserGivenDislikesAsync(int userId, int limit = 50, int offset = 0);
    Task<IEnumerable<Dislike>> GetUserReceivedDislikesAsync(int userId, int limit = 50, int offset = 0);
    Task<bool> CheckIfExistsAsync(int giverId, int receiverId);
}

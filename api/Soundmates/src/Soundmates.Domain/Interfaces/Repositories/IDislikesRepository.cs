using Soundmates.Domain.Entities;

namespace Soundmates.Domain.Interfaces.Repositories;

public interface IDislikesRepository : IBaseRepository<Dislike>
{
    Task<IEnumerable<Dislike>> GetUserGivenDislikesAsync(int userId, int limit = 50, int offset = 0);
    Task<IEnumerable<Dislike>> GetUserReceivedDislikesAsync(int userId, int limit = 50, int offset = 0);
}

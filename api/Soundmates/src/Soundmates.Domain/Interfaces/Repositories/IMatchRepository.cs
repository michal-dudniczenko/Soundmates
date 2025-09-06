using Soundmates.Domain.Entities;

namespace Soundmates.Domain.Interfaces.Repositories;

public interface IMatchRepository : IBaseRepository<Match>
{
    Task<IEnumerable<Match>> GetUserMatchesAsync(int userId, int limit, int offset);
    Task<bool> CheckIfMatchExistsAsync(int user1Id, int user2Id);
}

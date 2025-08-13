using Soundmates.Domain.Entities;

namespace Soundmates.Domain.Interfaces.Repositories;

public interface IMatchRepository : IBaseRepository<Match>
{
    Task<IEnumerable<Match>> GetUserMatchesAsync(int userId, int limit = 50, int offset = 0);
}

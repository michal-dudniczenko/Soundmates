using Soundmates.Domain.Entities;

namespace Soundmates.Domain.Interfaces.Repositories;

public interface IMatchRepository
{
    Task<IEnumerable<Match>> GetUserMatchesAsync(Guid userId, int limit, int offset);
    Task AddAsync(Match entity);
    Task<bool> CheckIfMatchExistsAsync(Guid user1Id, Guid user2Id);
}

using Soundmates.Domain.Entities;
using Soundmates.Domain.Interfaces.Repositories;

namespace Soundmates.Infrastructure.Repositories;

public class MatchRepository : IMatchRepository
{
    public Task AddAsync(Match entity)
    {
        throw new NotImplementedException();
    }

    public Task AddDislikeAsync(Dislike entity)
    {
        throw new NotImplementedException();
    }

    public Task AddLikeAsync(Like entity)
    {
        throw new NotImplementedException();
    }

    public Task<bool> CheckIfLikeExistsAsync(Guid giverId, Guid receiverId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> CheckIfMatchExistsAsync(Guid user1Id, Guid user2Id)
    {
        throw new NotImplementedException();
    }

    public Task<bool> CheckIfReactionExistsAsync(Guid giverId, Guid receiverId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Match>> GetUserMatchesAsync(Guid userId, int limit, int offset)
    {
        throw new NotImplementedException();
    }
}

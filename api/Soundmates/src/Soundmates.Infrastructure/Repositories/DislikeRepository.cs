using Soundmates.Domain.Entities;
using Soundmates.Domain.Interfaces.Repositories;

namespace Soundmates.Infrastructure.Repositories;

public class DislikeRepository : IDislikeRepository
{
    public Task AddAsync(Dislike entity)
    {
        throw new NotImplementedException();
    }

    public Task<bool> CheckIfExistsAsync(Guid giverId, Guid receiverId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Dislike>> GetUserGivenDislikesAsync(Guid userId, int limit, int offset)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Dislike>> GetUserReceivedDislikesAsync(Guid userId, int limit, int offset)
    {
        throw new NotImplementedException();
    }
}

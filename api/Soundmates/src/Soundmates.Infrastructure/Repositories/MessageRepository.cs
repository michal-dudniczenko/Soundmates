using Soundmates.Domain.Entities;
using Soundmates.Domain.Interfaces.Repositories;

namespace Soundmates.Infrastructure.Repositories;

public class MessageRepository : IMessageRepository
{
    public Task AddAsync(Message entity)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Message>> GetConversationAsync(Guid user1Id, Guid user2Id, int limit, int offset)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Message>> GetConversationsPreviewAsync(Guid userId, int limit, int offset)
    {
        throw new NotImplementedException();
    }
}

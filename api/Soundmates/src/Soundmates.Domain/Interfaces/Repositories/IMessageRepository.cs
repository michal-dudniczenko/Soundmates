using Soundmates.Domain.Entities;

namespace Soundmates.Domain.Interfaces.Repositories;

public interface IMessageRepository : IBaseRepository<Message>
{
    Task<IEnumerable<Message>> GetConversationAsync(Guid user1Id, Guid user2Id, int limit, int offset);
    Task<IEnumerable<Message>> GetConversationsLastMessagesAsync(Guid userId, int limit, int offset);
}

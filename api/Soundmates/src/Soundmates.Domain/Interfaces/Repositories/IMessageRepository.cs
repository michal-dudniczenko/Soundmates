using Soundmates.Domain.Entities;

namespace Soundmates.Domain.Interfaces.Repositories;

public interface IMessageRepository : IBaseRepository<Message>
{
    Task<IEnumerable<Message>> GetConversationAsync(int user1Id, int user2Id, int limit, int offset);
    Task<IEnumerable<Message>> GetConversationsLastMessagesAsync(int userId, int limit, int offset);
}

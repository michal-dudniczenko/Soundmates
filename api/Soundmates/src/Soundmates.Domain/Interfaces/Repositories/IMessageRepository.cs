using Soundmates.Domain.Entities;

namespace Soundmates.Domain.Interfaces.Repositories;

public interface IMessageRepository
{
    Task<IEnumerable<Message>> GetConversationAsync(Guid user1Id, Guid user2Id, int limit, int offset);
    Task<IEnumerable<Message>> GetConversationsPreviewAsync(Guid userId);
    Task AddAsync(Message entity);
}

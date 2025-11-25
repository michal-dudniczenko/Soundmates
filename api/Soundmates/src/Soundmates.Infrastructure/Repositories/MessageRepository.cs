using Microsoft.EntityFrameworkCore;
using Soundmates.Domain.Entities;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Infrastructure.Database;

namespace Soundmates.Infrastructure.Repositories;

public class MessageRepository(
    ApplicationDbContext _context
) : IMessageRepository
{
    public async Task AddAsync(Message entity)
    {
        _context.Messages.Add(entity);

        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Message>> GetConversationAsync(Guid user1Id, Guid user2Id, int limit, int offset)
    {
        return await _context.Messages
            .AsNoTracking()
            .Where(m => (m.SenderId == user1Id && m.ReceiverId == user2Id)
                || (m.SenderId == user2Id && m.ReceiverId == user1Id))
            .OrderBy(m => m.Timestamp)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<IEnumerable<Message>> GetConversationsPreviewAsync(Guid userId)
    {
        //var latestMessages = await _context.Messages
        //    .AsNoTracking()
        //    .Where(m => m.SenderId == userId || m.ReceiverId == userId)
        //    .GroupBy(m => new
        //    {
        //        User1Id = m.SenderId < m.ReceiverId ? m.SenderId : m.ReceiverId,
        //        User2Id = m.SenderId < m.ReceiverId ? m.ReceiverId : m.SenderId
        //    })
        //    .Select(g => g.OrderByDescending(m => m.Timestamp).First())
        //    .OrderByDescending(m => m.Timestamp)
        //    .ToListAsync();

        var userMessages = await _context.Messages
            .AsNoTracking()
            .Where(m => m.SenderId == userId || m.ReceiverId == userId)
            .ToListAsync();

        var conversationsGroups = userMessages
            .GroupBy(m => new
            {
                User1Id = m.SenderId < m.ReceiverId ? m.SenderId : m.ReceiverId,
                User2Id = m.SenderId < m.ReceiverId ? m.ReceiverId : m.SenderId
            });

        var latestMessages = conversationsGroups
            .Select(g => g.OrderByDescending(m => m.Timestamp).First());

        return latestMessages
            .OrderByDescending(m => m.Timestamp);
    }

    public async Task ReadConversation(Guid readerId, Guid otherUserId)
    {
        await _context.Messages
            .Where(m => m.ReceiverId == readerId && m.SenderId == otherUserId && !m.IsSeen)
            .ExecuteUpdateAsync(s => s
                .SetProperty(m => m.IsSeen, true));
    }
}

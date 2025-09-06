using Microsoft.EntityFrameworkCore;
using Soundmates.Domain.Entities;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Infrastructure.Database;
using Soundmates.Infrastructure.Repositories.Utils;

namespace Soundmates.Infrastructure.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly ApplicationDbContext _context;

    public MessageRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Message?> GetByIdAsync(Guid entityId)
    {
        return await _context.Messages
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == entityId);
    }

    public async Task<IEnumerable<Message>> GetAllAsync(int limit, int offset)
    {
        RepositoryUtils.ValidateLimitOffset(limit: limit, offset: offset);

        return await _context.Messages
            .AsNoTracking()
            .OrderBy(e => e.Id)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<bool> CheckIfExistsAsync(Guid entityId)
    {
        return await _context.Messages.AnyAsync(e => e.Id == entityId);
    }

    public async Task<Guid> AddAsync(Message entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        _context.Messages.Add(entity);
        await _context.SaveChangesAsync();

        return entity.Id;
    }

    public async Task<bool> UpdateAsync(Message entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        _context.Messages.Update(entity);
        var affected = await _context.SaveChangesAsync();

        return affected > 0;
    }

    public async Task<bool> RemoveAsync(Guid entityId)
    {
        var entity = await _context.Messages.FindAsync(entityId);

        if (entity is null) return false;

        _context.Messages.Remove(entity);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<Message>> GetConversationAsync(Guid user1Id, Guid user2Id, int limit, int offset)
    {
        RepositoryUtils.ValidateLimitOffset(limit: limit, offset: offset);

        return await _context.Messages
            .AsNoTracking()
            .Where(e => (e.SenderId == user1Id && e.ReceiverId == user2Id) || (e.SenderId == user2Id && e.ReceiverId == user1Id))
            .OrderByDescending(e => e.Timestamp)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<IEnumerable<Message>> GetConversationsLastMessagesAsync(Guid userId, int limit, int offset)
    {
        RepositoryUtils.ValidateLimitOffset(limit: limit, offset: offset);

        return await _context.Messages
            .AsNoTracking()
            .Where(m => m.SenderId == userId || m.ReceiverId == userId)
            .Select(m => new
            {
                Message = m,
                UserA = m.SenderId < m.ReceiverId ? m.SenderId : m.ReceiverId,
                UserB = m.SenderId < m.ReceiverId ? m.ReceiverId : m.SenderId
            })
            .GroupBy(x => new { x.UserA, x.UserB })
            .Select(g => g.OrderByDescending(x => x.Message.Timestamp).First().Message)
            .OrderByDescending(m => m.Timestamp)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();
    }
}

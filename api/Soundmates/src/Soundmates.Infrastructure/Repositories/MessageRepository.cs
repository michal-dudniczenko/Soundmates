using Microsoft.EntityFrameworkCore;
using Soundmates.Domain.Entities;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Infrastructure.Database;

namespace Soundmates.Infrastructure.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly ApplicationDbContext _context;

    public MessageRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Message?> GetByIdAsync(int entityId)
    {
        return await _context.Messages
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == entityId);
    }

    public async Task<IEnumerable<Message>> GetAllAsync(int limit = 50, int offset = 0)
    {
        RepositoryUtils.ValidateLimitOffset(limit: limit, offset: offset);

        return await _context.Messages
            .AsNoTracking()
            .OrderBy(e => e.Id)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();
    }

    public async Task AddAsync(Message entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await _context.Messages.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveAsync(int entityId)
    {
        var entity = await _context.Messages.FindAsync(entityId)
            ?? throw new KeyNotFoundException(RepositoryUtils.GetKeyNotFoundMessage<Message>(entityId: entityId));

        _context.Messages.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Message entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var exists = await _context.Messages.AnyAsync(e => e.Id == entity.Id);

        if (!exists)
        {
            throw new KeyNotFoundException(RepositoryUtils.GetKeyNotFoundMessage<Message>(entityId: entity.Id));
        }

        _context.Messages.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Message>> GetConversationAsync(int user1Id, int user2Id, int limit = 50, int offset = 0)
    {
        RepositoryUtils.ValidateLimitOffset(limit: limit, offset: offset);

        var user1Exists = await _context.Messages.AnyAsync(e => e.Id == user1Id);

        if (!user1Exists)
        {
            throw new KeyNotFoundException(RepositoryUtils.GetKeyNotFoundMessage<User>(entityId: user1Id));
        }

        var user2Exists = await _context.Messages.AnyAsync(e => e.Id == user2Id);

        if (!user2Exists)
        {
            throw new KeyNotFoundException(RepositoryUtils.GetKeyNotFoundMessage<User>(entityId: user2Id));
        }

        return await _context.Messages
            .AsNoTracking()
            .Where(e => (e.SenderId == user1Id && e.ReceiverId == user2Id) || (e.SenderId == user2Id && e.ReceiverId == user1Id))
            .OrderByDescending(e => e.Timestamp)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<IEnumerable<Message>> GetConversationsLastMessagesAsync(int userId, int limit = 50, int offset = 0)
    {
        RepositoryUtils.ValidateLimitOffset(limit: limit, offset: offset);

        var exists = await _context.Users.AnyAsync(e => e.Id == userId);

        if (!exists)
        {
            throw new KeyNotFoundException(RepositoryUtils.GetKeyNotFoundMessage<User>(entityId: userId));
        }

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

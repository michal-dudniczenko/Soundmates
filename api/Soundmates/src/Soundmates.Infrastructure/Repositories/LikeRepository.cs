using Microsoft.EntityFrameworkCore;
using Soundmates.Domain.Entities;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Infrastructure.Database;

namespace Soundmates.Infrastructure.Repositories;

public class LikeRepository : ILikeRepository
{
    private readonly ApplicationDbContext _context;

    public LikeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Like?> GetByIdAsync(int entityId)
    {
        return await _context.Likes.FindAsync(entityId);
    }

    public async Task<IEnumerable<Like>> GetAllAsync(int limit = 50, int offset = 0)
    {
        RepositoryUtils.ValidateLimitOffset(limit: limit, offset: offset);

        return await _context.Likes
            .AsNoTracking()
            .OrderBy(e => e.Id)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();
    }

    public async Task AddAsync(Like entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await _context.Likes.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveAsync(int entityId)
    {
        var entity = await _context.Likes.FindAsync(entityId)
            ?? throw new KeyNotFoundException(RepositoryUtils.GetKeyNotFoundMessage<Like>(entityId: entityId));

        _context.Likes.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Like entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var exists = await _context.Likes.AnyAsync(e => e.Id == entity.Id);

        if (!exists)
        {
            throw new KeyNotFoundException(RepositoryUtils.GetKeyNotFoundMessage<Like>(entityId: entity.Id));
        }

        _context.Likes.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Like>> GetUserGivenLikesAsync(int userId, int limit = 50, int offset = 0)
    {
        RepositoryUtils.ValidateLimitOffset(limit: limit, offset: offset);

        var exists = await _context.Users.AnyAsync(e => e.Id == userId);

        if (!exists)
        {
            throw new KeyNotFoundException(RepositoryUtils.GetKeyNotFoundMessage<User>(entityId: userId));
        }

        return await _context.Likes
            .AsNoTracking()
            .Where(e => e.GiverId == userId)
            .OrderBy(e => e.Id)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<IEnumerable<Like>> GetUserReceivedLikesAsync(int userId, int limit = 50, int offset = 0)
    {
        RepositoryUtils.ValidateLimitOffset(limit: limit, offset: offset);

        var exists = await _context.Users.AnyAsync(e => e.Id == userId);

        if (!exists)
        {
            throw new KeyNotFoundException(RepositoryUtils.GetKeyNotFoundMessage<User>(entityId: userId));
        }

        return await _context.Likes
            .AsNoTracking()
            .Where(e => e.ReceiverId == userId)
            .OrderBy(e => e.Id)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<bool> CheckIfExistsAsync(int giverId, int receiverId)
    {
        return await _context.Likes.AnyAsync(e => e.GiverId == giverId && e.ReceiverId == receiverId);
    }
}

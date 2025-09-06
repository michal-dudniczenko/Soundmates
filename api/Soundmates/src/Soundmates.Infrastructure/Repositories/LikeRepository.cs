using Microsoft.EntityFrameworkCore;
using Soundmates.Domain.Entities;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Infrastructure.Database;
using Soundmates.Infrastructure.Repositories.Utils;

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
        return await _context.Likes
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == entityId);
    }

    public async Task<IEnumerable<Like>> GetAllAsync(int limit, int offset)
    {
        RepositoryUtils.ValidateLimitOffset(limit: limit, offset: offset);

        return await _context.Likes
            .AsNoTracking()
            .OrderBy(e => e.Id)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<bool> CheckIfExistsAsync(int entityId)
    {
        return await _context.Likes.AnyAsync(e => e.Id == entityId);
    }

    public async Task<int> AddAsync(Like entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        _context.Likes.Add(entity);
        await _context.SaveChangesAsync();

        return entity.Id;
    }

    public async Task<bool> UpdateAsync(Like entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        _context.Likes.Update(entity);
        var affected = await _context.SaveChangesAsync();

        return affected > 0;
    }

    public async Task<bool> RemoveAsync(int entityId)
    {
        var entity = await _context.Likes.FindAsync(entityId);

        if (entity is null) return false;

        _context.Likes.Remove(entity);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<Like>> GetUserGivenLikesAsync(int userId, int limit, int offset)
    {
        RepositoryUtils.ValidateLimitOffset(limit: limit, offset: offset);

        return await _context.Likes
            .AsNoTracking()
            .Where(e => e.GiverId == userId)
            .OrderBy(e => e.Id)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<IEnumerable<Like>> GetUserReceivedLikesAsync(int userId, int limit, int offset)
    {
        RepositoryUtils.ValidateLimitOffset(limit: limit, offset: offset);

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

using Microsoft.EntityFrameworkCore;
using Soundmates.Domain.Entities;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Infrastructure.Database;
using Soundmates.Infrastructure.Repositories.Utils;

namespace Soundmates.Infrastructure.Repositories;

public class DislikeRepository : IDislikeRepository
{
    private readonly ApplicationDbContext _context;

    public DislikeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Dislike?> GetByIdAsync(int entityId)
    {
        return await _context.Dislikes
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == entityId);
    }

    public async Task<IEnumerable<Dislike>> GetAllAsync(int limit, int offset)
    {
        RepositoryUtils.ValidateLimitOffset(limit: limit, offset: offset);

        return await _context.Dislikes
            .AsNoTracking()
            .OrderBy(e => e.Id)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<bool> CheckIfExistsAsync(int entityId)
    {
        return await _context.Dislikes.AnyAsync(e => e.Id == entityId);
    }

    public async Task<int> AddAsync(Dislike entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        _context.Dislikes.Add(entity);
        await _context.SaveChangesAsync();

        return entity.Id;
    }

    public async Task<bool> UpdateAsync(Dislike entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        _context.Dislikes.Update(entity);
        var affected = await _context.SaveChangesAsync();

        return affected > 0;
    }

    public async Task<bool> RemoveAsync(int entityId)
    {
        var entity = await _context.Dislikes.FindAsync(entityId);

        if (entity is null) return false;

        _context.Dislikes.Remove(entity);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<Dislike>> GetUserGivenDislikesAsync(int userId, int limit, int offset)
    {
        RepositoryUtils.ValidateLimitOffset(limit: limit, offset: offset);

        return await _context.Dislikes
            .AsNoTracking()
            .Where(e => e.GiverId == userId)
            .OrderBy(e => e.Id)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<IEnumerable<Dislike>> GetUserReceivedDislikesAsync(int userId, int limit, int offset)
    {
        RepositoryUtils.ValidateLimitOffset(limit: limit, offset: offset);

        return await _context.Dislikes
            .AsNoTracking()
            .Where(e => e.ReceiverId == userId)
            .OrderBy(e => e.Id)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<bool> CheckIfExistsAsync(int giverId, int receiverId)
    {
        return await _context.Dislikes.AnyAsync(e => e.GiverId == giverId && e.ReceiverId == receiverId);
    }
}

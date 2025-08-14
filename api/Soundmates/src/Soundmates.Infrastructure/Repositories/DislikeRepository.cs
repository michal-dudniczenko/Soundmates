using Microsoft.EntityFrameworkCore;
using Soundmates.Domain.Entities;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Infrastructure.Database;

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
        return await _context.Dislikes.FindAsync(entityId);
    }

    public async Task<IEnumerable<Dislike>> GetAllAsync(int limit = 50, int offset = 0)
    {
        RepositoryUtils.ValidateLimitOffset(limit: limit, offset: offset);

        return await _context.Dislikes
            .AsNoTracking()
            .OrderBy(e => e.Id)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();
    }

    public async Task AddAsync(Dislike entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await _context.Dislikes.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Dislike entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var exists = await _context.Dislikes.AnyAsync(e => e.Id == entity.Id);

        if (!exists)
        {
            throw new KeyNotFoundException(RepositoryUtils.GetKeyNotFoundMessage<Dislike>(entityId: entity.Id));
        }

        _context.Dislikes.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveAsync(int entityId)
    {
        var entity = await _context.Dislikes.FindAsync(entityId)
            ?? throw new KeyNotFoundException(RepositoryUtils.GetKeyNotFoundMessage<Dislike>(entityId: entityId));

        _context.Dislikes.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Dislike>> GetUserGivenDislikesAsync(int userId, int limit = 50, int offset = 0)
    {
        RepositoryUtils.ValidateLimitOffset(limit: limit, offset: offset);

        var exists = await _context.Users.AnyAsync(e => e.Id == userId);

        if (!exists)
        {
            throw new KeyNotFoundException(RepositoryUtils.GetKeyNotFoundMessage<User>(entityId: userId));
        }

        return await _context.Dislikes
            .AsNoTracking()
            .Where(e => e.GiverId == userId)
            .OrderBy(e => e.Id)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<IEnumerable<Dislike>> GetUserReceivedDislikesAsync(int userId, int limit = 50, int offset = 0)
    {
        RepositoryUtils.ValidateLimitOffset(limit: limit, offset: offset);

        var exists = await _context.Users.AnyAsync(e => e.Id == userId);

        if (!exists)
        {
            throw new KeyNotFoundException(RepositoryUtils.GetKeyNotFoundMessage<User>(entityId: userId));
        }

        return await _context.Dislikes
            .AsNoTracking()
            .Where(e => e.ReceiverId == userId)
            .OrderBy(e => e.Id)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();
    }
}

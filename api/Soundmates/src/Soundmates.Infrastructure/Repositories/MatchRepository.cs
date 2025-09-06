using Microsoft.EntityFrameworkCore;
using Soundmates.Domain.Entities;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Infrastructure.Database;
using Soundmates.Infrastructure.Repositories.Utils;

namespace Soundmates.Infrastructure.Repositories;

public class MatchRepository : IMatchRepository
{
    private readonly ApplicationDbContext _context;

    public MatchRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Match?> GetByIdAsync(int entityId)
    {
        return await _context.Matches
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == entityId);
    }

    public async Task<IEnumerable<Match>> GetAllAsync(int limit, int offset)
    {
        RepositoryUtils.ValidateLimitOffset(limit: limit, offset: offset);

        return await _context.Matches
            .AsNoTracking()
            .OrderBy(e => e.Id)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<bool> CheckIfExistsAsync(int entityId)
    {
        return await _context.Matches.AnyAsync(e => e.Id == entityId);
    }

    public async Task<int> AddAsync(Match entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var existing = await _context.Matches.FirstOrDefaultAsync(e => 
            (e.User1Id == entity.User1Id && e.User2Id == entity.User2Id) || 
            (e.User1Id == entity.User2Id && e.User2Id == entity.User1Id));

        if (existing is not null)
        {
            return existing.Id;
        }

        _context.Matches.Add(entity);
        await _context.SaveChangesAsync();

        return entity.Id;
    }

    public async Task<bool> UpdateAsync(Match entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        _context.Matches.Update(entity);
        var affected = await _context.SaveChangesAsync();

        return affected > 0;
    }

    public async Task<bool> RemoveAsync(int entityId)
    {
        var entity = await _context.Matches.FindAsync(entityId);

        if (entity is null) return false;

        _context.Matches.Remove(entity);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<Match>> GetUserMatchesAsync(int userId, int limit, int offset)
    {
        RepositoryUtils.ValidateLimitOffset(limit: limit, offset: offset);

        return await _context.Matches
            .AsNoTracking()
            .Where(e => e.User1Id == userId || e.User2Id == userId)
            .OrderBy(e => e.Id)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<bool> CheckIfMatchExistsAsync(int user1Id, int user2Id)
    {
        return await _context.Matches.AnyAsync(m => (m.User1Id == user1Id && m.User2Id == user2Id) ||
            (m.User1Id == user2Id && m.User2Id == user1Id));
    }
}

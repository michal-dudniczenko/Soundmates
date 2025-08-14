using Microsoft.EntityFrameworkCore;
using Soundmates.Domain.Entities;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Infrastructure.Database;

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
        return await _context.Matches.FindAsync(entityId);
    }

    public async Task<IEnumerable<Match>> GetAllAsync(int limit = 50, int offset = 0)
    {
        RepositoryUtils.ValidateLimitOffset(limit: limit, offset: offset);

        return await _context.Matches
            .AsNoTracking()
            .OrderBy(e => e.Id)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();
    }

    public async Task AddAsync(Match entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await _context.Matches.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveAsync(int entityId)
    {
        var entity = await _context.Matches.FindAsync(entityId)
            ?? throw new KeyNotFoundException(RepositoryUtils.GetKeyNotFoundMessage<Match>(entityId: entityId));

        _context.Matches.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Match entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var exists = await _context.Matches.AnyAsync(e => e.Id == entity.Id);

        if (!exists)
        {
            throw new KeyNotFoundException(RepositoryUtils.GetKeyNotFoundMessage<Match>(entityId: entity.Id));
        }

        _context.Matches.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Match>> GetUserMatchesAsync(int userId, int limit = 50, int offset = 0)
    {
        RepositoryUtils.ValidateLimitOffset(limit: limit, offset: offset);

        var exists = await _context.Users.AnyAsync(e => e.Id == userId);

        if (!exists)
        {
            throw new KeyNotFoundException(RepositoryUtils.GetKeyNotFoundMessage<User>(entityId: userId));
        }

        return await _context.Matches
            .AsNoTracking()
            .Where(e => e.User1Id == userId || e.User2Id == userId)
            .OrderBy(e => e.Id)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();
    }
}

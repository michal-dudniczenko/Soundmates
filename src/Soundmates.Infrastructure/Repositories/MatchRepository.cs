using Microsoft.EntityFrameworkCore;
using Soundmates.Domain.Entities;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Infrastructure.Database;

namespace Soundmates.Infrastructure.Repositories;

public class MatchRepository(
    ApplicationDbContext _context
) : IMatchRepository
{
    public async Task AddMatchAsync(Match entity)
    {
        _context.Matches.Add(entity);

        await _context.SaveChangesAsync();
    }

    public async Task AddDislikeAsync(Dislike entity)
    {
        _context.Dislikes.Add(entity);

        await _context.SaveChangesAsync();
    }

    public async Task AddLikeAsync(Like entity)
    {
        _context.Likes.Add(entity);

        await _context.SaveChangesAsync();
    }

    public async Task<bool> CheckIfLikeExistsAsync(Guid giverId, Guid receiverId)
    {
        return await _context.Likes.AnyAsync(l => l.GiverId == giverId && l.ReceiverId == receiverId);
    }

    public async Task<bool> CheckIfMatchExistsAsync(Guid user1Id, Guid user2Id)
    {
        return await _context.Matches.AnyAsync(m => (m.User1Id == user1Id && m.User2Id == user2Id) 
        || (m.User1Id == user2Id && m.User2Id == user1Id));
    }

    public async Task<bool> CheckIfDislikeExistsAsync(Guid giverId, Guid receiverId)
    {
        return await _context.Dislikes.AnyAsync(dl => dl.GiverId == giverId && dl.ReceiverId == receiverId);
    }

    public async Task<IEnumerable<Match>> GetUserMatchesAsync(Guid userId, int limit, int offset)
    {
        return await _context.Matches
            .AsNoTracking()
            .Include(m => m.User1)
            .Include(m => m.User2)
            .Where(m => m.User1Id == userId || m.User2Id == userId)
            .OrderBy(m => m.Id)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();
    }

    public async Task DeleteMatchAsync(Guid user1Id, Guid user2Id)
    {
        await _context.Matches
            .Where(m => (m.User1Id == user1Id && m.User2Id == user2Id)
                || (m.User1Id == user2Id && m.User2Id == user1Id))
            .ExecuteDeleteAsync();
    }
}

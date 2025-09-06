using Microsoft.EntityFrameworkCore;
using Soundmates.Domain.Entities;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Infrastructure.Database;
using Soundmates.Infrastructure.Repositories.Utils;

namespace Soundmates.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(int entityId)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == entityId);
    }

    public async Task<IEnumerable<User>> GetAllAsync(int limit, int offset)
    {
        RepositoryUtils.ValidateLimitOffset(limit: limit, offset: offset);

        return await _context.Users
            .AsNoTracking()
            .OrderBy(e => e.Id)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<bool> CheckIfExistsAsync(int entityId)
    {
        return await _context.Users.AnyAsync(e => e.Id == entityId);
    }

    public async Task<int> AddAsync(User entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        _context.Users.Add(entity);
        await _context.SaveChangesAsync();

        return entity.Id;
    }

    public async Task<bool> UpdateAsync(User entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        _context.Users.Update(entity);
        var affected = await _context.SaveChangesAsync();

        return affected > 0;
    }

    public async Task<bool> RemoveAsync(int entityId)
    {
        var entity = await _context.Users.FindAsync(entityId);

        if (entity is null) return false;

        _context.Users.Remove(entity);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Email == email);
    }

    public async Task<IEnumerable<User>> GetAllActiveAsync(int limit, int offset)
    {
        RepositoryUtils.ValidateLimitOffset(limit: limit, offset: offset);

        return await _context.Users
            .AsNoTracking()
            .Where(e => 
                e.IsActive && 
                !e.IsFirstLogin &&
                e.IsEmailConfirmed
            )
            .OrderBy(e => e.Id)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<IEnumerable<User>> GetPotentialMatchesAsync(int userId, int limit, int offset)
    {
        RepositoryUtils.ValidateLimitOffset(limit: limit, offset: offset);

        return await _context.Users
            .AsNoTracking()
            .Where(otherUser =>
                otherUser.Id != userId &&
                otherUser.IsActive &&
                !otherUser.IsFirstLogin &&
                otherUser.IsEmailConfirmed &&
                !_context.Likes.Any(l => l.GiverId == userId && l.ReceiverId == otherUser.Id) &&
                !_context.Dislikes.Any(d => d.GiverId == userId && d.ReceiverId == otherUser.Id)
            )
            .OrderBy(e => e.Id)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<bool> DeactivateUserAccountAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user is null) return false;

        user.IsActive = false;
        user.RefreshTokenHash = null;
        user.RefreshTokenExpiresAt = null;
        user.IsLoggedOut = true;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UpdateUserPasswordAsync(int userId, string newPasswordHash)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user is null) return false;

        user.PasswordHash = newPasswordHash;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> LogInUserAsync(int userId, string newRefreshTokenHash, DateTime newRefreshTokenExpiresAt)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user is null) return false;

        user.RefreshTokenHash = newRefreshTokenHash;
        user.RefreshTokenExpiresAt = newRefreshTokenExpiresAt;
        user.IsLoggedOut = false;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> LogOutUserAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user is null) return false;

        user.RefreshTokenHash = null;
        user.RefreshTokenExpiresAt = null;
        user.IsLoggedOut = true;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> CheckIfEmailExistsAsync(string email)
    {
        return await _context.Users.AnyAsync(e => e.Email == email);
    }

    public async Task<int?> CheckRefreshTokenGetUserIdAsync(string refreshTokenHash)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.RefreshTokenHash == refreshTokenHash);

        if (user is null || user.RefreshTokenExpiresAt < DateTime.UtcNow)
        {
            return null;
        }

        return user.Id;
    }

    public async Task<bool> CheckIfExistsActiveAsync(int userId)
    {
        return await _context.Users.AnyAsync(e => e.Id == userId && e.IsActive && e.IsEmailConfirmed && !e.IsFirstLogin);
    }
}

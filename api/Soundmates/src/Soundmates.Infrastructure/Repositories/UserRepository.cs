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

    public async Task<User?> GetByIdAsync(Guid entityId)
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

    public async Task<bool> CheckIfExistsAsync(Guid entityId)
    {
        return await _context.Users.AnyAsync(e => e.Id == entityId);
    }

    public async Task<Guid> AddAsync(User entity)
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

    public async Task<bool> RemoveAsync(Guid entityId)
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

    public async Task<IEnumerable<User>> GetPotentialMatchesAsync(Guid userId, int limit, int offset)
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

    public async Task<bool> DeactivateUserAccountAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user is null) return false;

        user.IsActive = false;
        user.IsLoggedOut = true;

        var refreshToken = await _context.RefreshTokens.FindAsync(user.Id);

        if (refreshToken is not null)
        {
            _context.RefreshTokens.Remove(refreshToken);
        }
        
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UpdateUserPasswordAsync(Guid userId, string newPasswordHash)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user is null) return false;

        user.PasswordHash = newPasswordHash;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> LogInUserAsync(Guid userId, string newRefreshTokenHash, DateTime newRefreshTokenExpiresAt)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user is null) return false;

        var existingRefreshToken = await _context.RefreshTokens.FindAsync(user.Id);

        if (existingRefreshToken is null)
        {
            var newRefreshToken = new RefreshToken
            {
                UserId = user.Id,
                RefreshTokenHash = newRefreshTokenHash,
                RefreshTokenExpiresAt = newRefreshTokenExpiresAt
            };

            _context.RefreshTokens.Add(newRefreshToken);
        }
        else
        {
            existingRefreshToken.RefreshTokenHash = newRefreshTokenHash;
            existingRefreshToken.RefreshTokenExpiresAt = newRefreshTokenExpiresAt;
        }

        user.IsLoggedOut = false;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> LogOutUserAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user is null) return false;

        user.IsLoggedOut = true;

        var refreshToken = await _context.RefreshTokens.FindAsync(user.Id);

        if (refreshToken is not null)
        {
            _context.RefreshTokens.Remove(refreshToken);
        }

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> CheckIfEmailExistsAsync(string email)
    {
        return await _context.Users.AnyAsync(e => e.Email == email);
    }

    public async Task<Guid?> CheckRefreshTokenGetUserIdAsync(string refreshTokenHash)
    {
        var refreshToken = await _context.RefreshTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.RefreshTokenHash == refreshTokenHash);

        if (refreshToken is null || refreshToken.RefreshTokenExpiresAt < DateTime.UtcNow)
        {
            return null;
        }

        return refreshToken.UserId;
    }

    public async Task<bool> CheckIfExistsActiveAsync(Guid userId)
    {
        return await _context.Users.AnyAsync(e => e.Id == userId && e.IsActive && e.IsEmailConfirmed && !e.IsFirstLogin);
    }
}

using Microsoft.EntityFrameworkCore;
using Soundmates.Domain.Entities;
using Soundmates.Domain.Interfaces.Auth;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Infrastructure.Database;

namespace Soundmates.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IAuthService _authService;

    public UserRepository(ApplicationDbContext context, IAuthService authService)
    {
        _context = context;
        _authService = authService;
    }

    public async Task<User?> GetByIdAsync(int entityId)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == entityId);
    }

    public async Task<IEnumerable<User>> GetAllAsync(int limit = 50, int offset = 0)
    {
        RepositoryUtils.ValidateLimitOffset(limit: limit, offset: offset);

        return await _context.Users
            .AsNoTracking()
            .OrderBy(e => e.Id)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();
    }

    public async Task AddAsync(User entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await _context.Users.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var exists = await _context.Users.AnyAsync(e => e.Id == entity.Id);

        if (!exists)
        {
            throw new KeyNotFoundException(RepositoryUtils.GetKeyNotFoundMessage<User>(entityId: entity.Id));
        }

        _context.Users.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveAsync(int entityId)
    {
        var entity = await _context.Users.FindAsync(entityId)
            ?? throw new KeyNotFoundException(RepositoryUtils.GetKeyNotFoundMessage<User>(entityId: entityId));

        _context.Users.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Email == email);
    }

    public async Task<IEnumerable<User>> GetAllActiveAsync(int limit = 50, int offset = 0)
    {
        RepositoryUtils.ValidateLimitOffset(limit: limit, offset: offset);

        return await _context.Users
            .AsNoTracking()
            .Where(e => e.IsActive && !e.IsFirstLogin)
            .OrderBy(e => e.Id)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<IEnumerable<User>> GetPotentialMatchesAsync(int userId, int limit = 50, int offset = 0)
    {
        RepositoryUtils.ValidateLimitOffset(limit: limit, offset: offset);

        return await _context.Users
            .AsNoTracking()
            .Where(otherUser =>
                otherUser.Id != userId &&
                otherUser.IsActive &&
                !otherUser.IsFirstLogin &&
                !_context.Likes.Any(l => l.GiverId == userId && l.ReceiverId == otherUser.Id) &&
                !_context.Dislikes.Any(d => d.GiverId == userId && d.ReceiverId == otherUser.Id)
            )
            .OrderBy(e => e.Id)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();
    }

    public async Task DeactivateUserAccountAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId)
            ?? throw new KeyNotFoundException(RepositoryUtils.GetKeyNotFoundMessage<User>(entityId: userId));

        user.IsActive = false;
        await _context.SaveChangesAsync();
        await LogOutUserAsync(userId);
    }

    public async Task UpdateUserPasswordAsync(int userId, string newPassword)
    {
        var user = await _context.Users.FindAsync(userId)
           ?? throw new KeyNotFoundException(RepositoryUtils.GetKeyNotFoundMessage<User>(entityId: userId));

        user.PasswordHash = _authService.GetPasswordHash(newPassword);
        await _context.SaveChangesAsync();
    }

    public async Task LogInUserAsync(int userId, string newRefreshToken)
    {
        var user = await _context.Users.FindAsync(userId)
            ?? throw new KeyNotFoundException(RepositoryUtils.GetKeyNotFoundMessage<User>(entityId: userId));

        user.RefreshTokenHash = _authService.GetRefreshTokenHash(newRefreshToken);
        user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(30);
        user.IsLoggedOut = false;
        await _context.SaveChangesAsync();
    }

    public async Task LogOutUserAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId)
            ?? throw new KeyNotFoundException(RepositoryUtils.GetKeyNotFoundMessage<User>(entityId: userId));

        user.RefreshTokenHash = null;
        user.RefreshTokenExpiresAt = null;
        user.IsLoggedOut = true;
        await _context.SaveChangesAsync();
    }

    public async Task<bool> CheckIfEmailExistsAsync(string email)
    {
        return await _context.Users.AnyAsync(e => e.Email == email);
    }

    public async Task<int?> CheckRefreshTokenGetUserIdAsync(string refreshToken)
    {
        var refreshTokenHash = _authService.GetRefreshTokenHash(refreshToken);

        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.RefreshTokenHash == refreshTokenHash);

        if (user is null || user.RefreshTokenExpiresAt < DateTime.UtcNow)
        {
            return null;
        }

        return user.Id;
    }
}

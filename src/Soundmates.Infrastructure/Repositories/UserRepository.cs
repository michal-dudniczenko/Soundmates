using Microsoft.EntityFrameworkCore;
using Soundmates.Domain.Entities;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Infrastructure.Database;

namespace Soundmates.Infrastructure.Repositories;

public class UserRepository(
    ApplicationDbContext _context
) : IUserRepository
{
    public async Task AddAsync(User entity)
    {
        _context.Users.Add(entity);

        await _context.SaveChangesAsync();
    }

    public async Task<bool> CheckIfEmailExistsAsync(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email == email);
    }

    public async Task<bool> CheckIfExistsActiveAsync(Guid userId)
    {
        return await _context.Users.AnyAsync(u => u.Id == userId && u.IsActive && u.IsEmailConfirmed && !u.IsFirstLogin);
    }

    public async Task<Guid?> CheckRefreshTokenGetUserIdAsync(string refreshTokenHash)
    {
        var refreshToken = await _context.RefreshTokens
            .AsNoTracking()
            .Where(rt => rt.RefreshTokenHash == refreshTokenHash && rt.RefreshTokenExpiresAt > DateTime.UtcNow)
            .FirstOrDefaultAsync();

        if (refreshToken is null) return null;

        return refreshToken.UserId;
    }

    public async Task DeactivateUserAccountAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId)
            ?? throw new InvalidOperationException($"User with id: {userId} was not found.");

        user.IsActive = false;
        user.IsLoggedOut = true;

        var existingRefreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.UserId == userId);

        if (existingRefreshToken is not null)
        {
            _context.RefreshTokens.Remove(existingRefreshToken);
        }

        await _context.SaveChangesAsync();
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByIdAsync(Guid entityId)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == entityId);
    }

    public async Task LogInUserAsync(Guid userId, string newRefreshTokenHash, DateTime newRefreshTokenExpiresAt)
    {
        var user = await _context.Users.FindAsync(userId)
            ?? throw new InvalidOperationException($"User with id: {userId} was not found.");

        user.IsLoggedOut = false;

        var existingRefreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.UserId == userId);

        if (existingRefreshToken is null)
        {
            _context.RefreshTokens.Add(new RefreshToken
            {
                RefreshTokenHash = newRefreshTokenHash,
                RefreshTokenExpiresAt = newRefreshTokenExpiresAt,
                UserId = userId
            });
        }
        else
        {
            existingRefreshToken.RefreshTokenHash = newRefreshTokenHash;
            existingRefreshToken.RefreshTokenExpiresAt = newRefreshTokenExpiresAt;
        }

        await _context.SaveChangesAsync();
    }

    public async Task LogOutUserAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId)
            ?? throw new InvalidOperationException($"User with id: {userId} was not found.");

        user.IsLoggedOut = true;

        var existingRefreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.UserId == userId);

        if (existingRefreshToken is not null)
        {
            _context.RefreshTokens.Remove(existingRefreshToken);
        }

        await _context.SaveChangesAsync();
    }

    public async Task UpdateUserPasswordAsync(Guid userId, string newPasswordHash)
    {
        var user = await _context.Users.FindAsync(userId)
            ?? throw new InvalidOperationException($"User with id: {userId} was not found.");

        user.PasswordHash = newPasswordHash;

        await _context.SaveChangesAsync();
    }
}

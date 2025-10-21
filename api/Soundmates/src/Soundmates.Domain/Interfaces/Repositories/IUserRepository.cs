using Soundmates.Domain.Entities;

namespace Soundmates.Domain.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid entityId);
    Task<User?> GetByEmailAsync(string email);
    Task AddAsync(User entity);
    Task DeactivateUserAccountAsync(Guid userId);
    Task UpdateUserPasswordAsync(Guid userId, string newPasswordHash);
    Task LogInUserAsync(Guid userId, string newRefreshTokenHash, DateTime newRefreshTokenExpiresAt);
    Task LogOutUserAsync(Guid userId);
    Task<bool> CheckIfEmailExistsAsync(string email);
    Task<Guid?> CheckRefreshTokenGetUserIdAsync(string refreshTokenHash);
    Task<bool> CheckIfExistsActiveAsync(Guid userId);
}

using Soundmates.Domain.Entities;

namespace Soundmates.Domain.Interfaces.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetAllActiveAsync(int limit, int offset);
    Task<IEnumerable<User>> GetPotentialMatchesAsync(Guid userId, int limit, int offset);
    Task<bool> DeactivateUserAccountAsync(Guid userId);
    Task<bool> UpdateUserPasswordAsync(Guid userId, string newPasswordHash);
    Task<bool> LogInUserAsync(Guid userId, string newRefreshTokenHash, DateTime newRefreshTokenExpiresAt);
    Task<bool> LogOutUserAsync(Guid userId);
    Task<bool> CheckIfEmailExistsAsync(string email);
    Task<Guid?> CheckRefreshTokenGetUserIdAsync(string refreshTokenHash);
    Task<bool> CheckIfExistsActiveAsync(Guid userId);
}

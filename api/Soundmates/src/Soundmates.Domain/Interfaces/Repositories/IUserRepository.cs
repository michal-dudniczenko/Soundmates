using Soundmates.Domain.Entities;

namespace Soundmates.Domain.Interfaces.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetAllActiveAsync(int limit, int offset);
    Task<IEnumerable<User>> GetPotentialMatchesAsync(int userId, int limit, int offset);
    Task<bool> DeactivateUserAccountAsync(int userId);
    Task<bool> UpdateUserPasswordAsync(int userId, string newPasswordHash);
    Task<bool> LogInUserAsync(int userId, string newRefreshTokenHash, DateTime newRefreshTokenExpiresAt);
    Task<bool> LogOutUserAsync(int userId);
    Task<bool> CheckIfEmailExistsAsync(string email);
    Task<int?> CheckRefreshTokenGetUserIdAsync(string refreshTokenHash);
    Task<bool> CheckIfExistsActiveAsync(int userId);
}

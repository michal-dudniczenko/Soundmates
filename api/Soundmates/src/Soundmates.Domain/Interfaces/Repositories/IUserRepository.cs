using Soundmates.Domain.Entities;

namespace Soundmates.Domain.Interfaces.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetAllActiveAsync(int limit = 50, int offset = 0);
    Task<IEnumerable<User>> GetPotentialMatchesAsync(int userId, int limit = 50, int offset = 0);
    Task DeactivateUserAccountAsync(int userId);
    Task UpdateUserPasswordAsync(int userId, string newPassword);
    Task LogInUserAsync(int userId, string newRefreshToken);
    Task LogOutUserAsync(int userId);
    Task<bool> CheckIfEmailExistsAsync(string email);
    Task<bool> CheckIfIdExistsAsync(int userId);
    Task<int?> CheckRefreshTokenGetUserIdAsync(string refreshToken);
}

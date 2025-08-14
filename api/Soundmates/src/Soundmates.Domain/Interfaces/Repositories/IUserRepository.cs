using Soundmates.Domain.Entities;

namespace Soundmates.Domain.Interfaces.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetAllActiveAsync(int limit = 50, int offset = 0);
    Task DeactivateUserAccountAsync(int userId);
    Task UpdateUserPasswordAsync(int userId, string newPassword);
    Task UpdateUserRefreshTokenAsync(int userId, string newRefreshToken);
    Task<bool> CheckIfEmailExistsAsync(string email);
}

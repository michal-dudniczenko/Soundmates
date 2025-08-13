using Soundmates.Domain.Entities;

namespace Soundmates.Domain.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int userId);
    Task<User?> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetAllAsync(int limit = 50, int offset = 0);
    Task<IEnumerable<User>> GetAllActiveAsync(int limit = 50, int offset = 0);
}

using Soundmates.Domain.Entities;

namespace Soundmates.Domain.Interfaces.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
    Task<IEnumerable<User>> GetActiveUsersAsync(int limit = 50, int offset = 0);
}

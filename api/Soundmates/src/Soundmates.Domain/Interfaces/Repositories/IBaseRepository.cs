namespace Soundmates.Domain.Interfaces.Repositories;

public interface IBaseRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int entityId);
    Task<IEnumerable<T>> GetAllAsync(int limit, int offset);
    Task<bool> CheckIfExistsAsync(int entityId);
    Task<int> AddAsync(T entity);
    Task<bool> UpdateAsync(T entity);
    Task<bool> RemoveAsync(int entityId);
}

namespace Soundmates.Domain.Interfaces.Repositories;

public interface IBaseRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int entityId);
    Task<IEnumerable<T>> GetAllAsync(int limit = 50, int offset = 0);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task RemoveAsync(int entityId);
}

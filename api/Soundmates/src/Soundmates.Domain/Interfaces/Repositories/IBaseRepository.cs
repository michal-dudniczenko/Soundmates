namespace Soundmates.Domain.Interfaces.Repositories;

public interface IBaseRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid entityId);
    Task<IEnumerable<T>> GetAllAsync(int limit, int offset);
    Task<bool> CheckIfExistsAsync(Guid entityId);
    Task<Guid> AddAsync(T entity);
    Task<bool> UpdateAsync(T entity);
    Task<bool> RemoveAsync(Guid entityId);
}

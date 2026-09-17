using RestaurantApp.Entity.Entities.Common;

namespace RestaurantApp.DataAccess.Repositories.Interfaces;

public interface IGenericRepository<T>
    where T : BaseEntity
{
    Task<List<T>> GetAllAsync();

    Task<T?> GetByIdAsync(int id);

    Task AddAsync(T entity);

    Task UpdateAsync(T entity);

    Task DeleteAsync(T entity);
}
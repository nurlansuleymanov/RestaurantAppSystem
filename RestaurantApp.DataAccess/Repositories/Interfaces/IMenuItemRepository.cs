using RestaurantApp.Entity.Entities;
using RestaurantApp.Entity.Entities.Enums;

namespace RestaurantApp.DataAccess.Repositories.Interfaces;

public interface IMenuItemRepository
    : IGenericRepository<MenuItem>
{
    Task<List<MenuItem>> GetByCategoryAsync(Category category);

    Task<List<MenuItem>> GetByPriceIntervalAsync(
        decimal minPrice,
        decimal maxPrice);

    Task<List<MenuItem>> SearchByNameAsync(string search);

    Task<bool> ExistsByNameAsync(string name);
}
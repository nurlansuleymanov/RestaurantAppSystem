using Microsoft.EntityFrameworkCore;
using RestaurantApp.DataAccess.Context;
using RestaurantApp.DataAccess.Repositories.Interfaces;
using RestaurantApp.Entity.Entities;
using RestaurantApp.Entity.Entities.Enums;

namespace RestaurantApp.DataAccess.Repositories.Implementations;

public class MenuItemRepository : GenericRepository<MenuItem>, IMenuItemRepository
{
    public MenuItemRepository(AppDbContext context)
        : base(context)
    {
    }

    public async Task<List<MenuItem>> GetByCategoryAsync(Category category)
    {
        return await _table
            .AsNoTracking()
            .Where(x => x.Category == category)
            .ToListAsync();
    }

    public async Task<List<MenuItem>> GetByPriceIntervalAsync( decimal minPrice,decimal maxPrice)
    {
        return await _table
            .AsNoTracking()
            .Where(x =>
                x.Price >= minPrice &&
                x.Price <= maxPrice)
            .ToListAsync();
    }

    public async Task<List<MenuItem>> SearchByNameAsync(string search)
    {
        return await _table
            .AsNoTracking()
            .Where(x => x.Name.Contains(search))
            .ToListAsync();
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await _table
            .AnyAsync(x => x.Name == name);
    }

    public async Task<bool> ExistsByNameAsync(
    string name,
    int? excludeId = null)
    {
        return await _table.AnyAsync(x =>
            x.Name == name &&
            (!excludeId.HasValue || x.Id != excludeId.Value));
    }
}
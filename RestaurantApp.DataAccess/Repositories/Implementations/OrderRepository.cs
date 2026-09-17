using Microsoft.EntityFrameworkCore;
using RestaurantApp.DataAccess.Context;
using RestaurantApp.DataAccess.Repositories.Interfaces;
using RestaurantApp.Entity.Entities;

namespace RestaurantApp.DataAccess.Repositories.Implementations;

public class OrderRepository : GenericRepository<Order>, IOrderRepository
{
    public OrderRepository(AppDbContext context)
        : base(context)
    {
    }

    public async Task<List<Order>> GetAllWithDetailsAsync()
    {
        return await _table
            .AsNoTracking()
            .Include(x => x.OrderItems)
            .ThenInclude(x => x.MenuItem)
            .ToListAsync();
    }

    public async Task<Order?> GetByIdWithDetailsAsync(int id)
    {
        return await _table
            .AsNoTracking()
            .Include(x => x.OrderItems)
            .ThenInclude(x => x.MenuItem)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<Order>> GetByDatesIntervalAsync( DateTime startDate, DateTime endDate)
    {
        return await _table
            .AsNoTracking()
            .Include(x => x.OrderItems)
            .ThenInclude(x => x.MenuItem)
            .Where(x =>
                x.Date >= startDate &&
                x.Date <= endDate)
            .ToListAsync();
    }

    public async Task<List<Order>> GetByDateAsync(DateTime date)
    {
        return await _table
            .AsNoTracking()
            .Include(x => x.OrderItems)
            .ThenInclude(x => x.MenuItem)
            .Where(x => x.Date.Date == date.Date)
            .ToListAsync();
    }

    public async Task<List<Order>> GetByPriceIntervalAsync( decimal minPrice, decimal maxPrice)
    {
        return await _table
            .AsNoTracking()
            .Include(x => x.OrderItems)
            .ThenInclude(x => x.MenuItem)
            .Where(x =>
                x.TotalAmount >= minPrice &&
                x.TotalAmount <= maxPrice)
            .ToListAsync();
    }
}
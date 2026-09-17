using RestaurantApp.Entity.Entities;

namespace RestaurantApp.DataAccess.Repositories.Interfaces;

public interface IOrderRepository : IGenericRepository<Order>
{
    Task<List<Order>> GetAllWithDetailsAsync();

    Task<Order?> GetByIdWithDetailsAsync(int id);

    Task<List<Order>> GetByDatesIntervalAsync( DateTime startDate, DateTime endDate);

    Task<List<Order>> GetByDateAsync(DateTime date);

    Task<List<Order>> GetByPriceIntervalAsync(decimal minPrice,decimal maxPrice);
}

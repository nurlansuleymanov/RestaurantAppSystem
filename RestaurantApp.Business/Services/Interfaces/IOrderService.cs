using RestaurantApp.Business.DTOs.Orders;

namespace RestaurantApp.Business.Services.Interfaces;

public interface IOrderService
{
    Task AddAsync(CreateOrderDto dto);

    Task UpdateAsync(int id, UpdateOrderDto dto);

    Task DeleteAsync(int id);

    Task<List<OrderDto>> GetAllAsync();

    Task<OrderDetailDto?> GetByIdAsync(int id);

    Task<List<OrderDto>> GetByDatesIntervalAsync(  DateTime startDate,DateTime endDate);

    Task<List<OrderDto>> GetByDateAsync(DateTime date);

    Task<List<OrderDto>> GetByPriceIntervalAsync(decimal minPrice, decimal maxPrice);
}

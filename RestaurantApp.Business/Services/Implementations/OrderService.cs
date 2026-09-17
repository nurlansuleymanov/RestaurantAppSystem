using AutoMapper;
using RestaurantApp.Business.DTOs.Orders;
using RestaurantApp.Business.Services.Interfaces;
using RestaurantApp.DataAccess.Repositories.Interfaces;
using RestaurantApp.Entity.Entities;

namespace RestaurantApp.Business.Services.Implementations;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMenuItemRepository _menuItemRepository;
    private readonly IMapper _mapper;

    public OrderService(
        IOrderRepository orderRepository,
        IMenuItemRepository menuItemRepository,
        IMapper mapper)
    {
        _orderRepository = orderRepository;
        _menuItemRepository = menuItemRepository;
        _mapper = mapper;
    }

    public async Task AddAsync(CreateOrderDto dto)
    {
        if (dto.OrderItems is null || dto.OrderItems.Count == 0)
            throw new Exception("Order item cannot be empty.");

        Order order = new Order
        {
            Date = DateTime.Now,
            OrderItems = new List<OrderItem>()
        };

        decimal totalAmount = 0;

        foreach (var itemDto in dto.OrderItems)
        {
            if (itemDto.Count <= 0)
                throw new Exception("Count must be greater than 0.");

            MenuItem? menuItem =
                await _menuItemRepository.GetByIdAsync(itemDto.MenuItemId);

            if (menuItem is null)
                throw new Exception(
                    $"Menu item with Id {itemDto.MenuItemId} not found.");

            OrderItem orderItem = new OrderItem
            {
                MenuItemId = menuItem.Id,
                MenuItem = menuItem,
                Count = itemDto.Count
            };

            order.OrderItems.Add(orderItem);

            totalAmount += menuItem.Price * itemDto.Count;
        }

        order.TotalAmount = totalAmount;

        await _orderRepository.AddAsync(order);
    }

    public async Task DeleteAsync(int id)
    {
        Order? order =
            await _orderRepository.GetByIdAsync(id);

        if (order is null)
            throw new Exception("Order not found.");

        await _orderRepository.DeleteAsync(order);
    }

    public async Task<List<OrderDto>> GetAllAsync()
    {
        List<Order> orders =
            await _orderRepository.GetAllWithDetailsAsync();

        return _mapper.Map<List<OrderDto>>(orders);
    }

    public async Task<OrderDetailDto?> GetByIdAsync(int id)
    {
        Order? order =
            await _orderRepository.GetByIdWithDetailsAsync(id);

        if (order is null)
            return null;

        return _mapper.Map<OrderDetailDto>(order);
    }

    public async Task<List<OrderDto>> GetByDatesIntervalAsync(
        DateTime startDate,
        DateTime endDate)
    {
        if (startDate > endDate)
            throw new Exception(
                "Start date cannot be greater than end date.");

        List<Order> orders =
            await _orderRepository
                .GetByDatesIntervalAsync(startDate, endDate);

        return _mapper.Map<List<OrderDto>>(orders);
    }

    public async Task<List<OrderDto>> GetByDateAsync(DateTime date)
    {
        List<Order> orders =
            await _orderRepository.GetByDateAsync(date);

        return _mapper.Map<List<OrderDto>>(orders);
    }

    public async Task<List<OrderDto>> GetByPriceIntervalAsync( decimal minPrice,  decimal maxPrice)
    {
        if (minPrice < 0 || maxPrice < 0)
            throw new Exception("Price cannot be negative.");

        if (minPrice > maxPrice)
            throw new Exception( "Minimum price cannot be greater than maximum price.");

        List<Order> orders = await _orderRepository
                .GetByPriceIntervalAsync(minPrice, maxPrice);

        return _mapper.Map<List<OrderDto>>(orders);
    }

    public async Task UpdateAsync(
        int id,
        UpdateOrderDto dto)
    {
      
        throw new NotImplementedException();
    }
}
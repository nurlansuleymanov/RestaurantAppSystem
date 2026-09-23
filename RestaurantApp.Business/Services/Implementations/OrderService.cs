using AutoMapper;
using RestaurantApp.Business.DTOs.Orders;
using RestaurantApp.Business.Exceptions;
using RestaurantApp.Business.Services.Interfaces;
using RestaurantApp.DataAccess.Repositories.Interfaces;
using RestaurantApp.Entity.Entities;

namespace RestaurantApp.Business.Services.Implementations;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMenuItemRepository _menuItemRepository;
    private readonly IMapper _mapper;

    public OrderService(IOrderRepository orderRepository,
        IMenuItemRepository menuItemRepository,IMapper mapper)
    {
        _orderRepository = orderRepository;
        _menuItemRepository = menuItemRepository;
        _mapper = mapper;
    }

    public async Task AddAsync(CreateOrderDto dto)
    {
        if (dto.OrderItems is null || dto.OrderItems.Count == 0)
            throw new ValidationException(
                "Order must contain at least one item.");

        var result = await PrepareOrderItemsAsync(dto.OrderItems);

        Order order = new Order
        {
            Date = DateTime.Now,
            TotalAmount = result.TotalAmount,
            OrderItems = result.OrderItems
        };

        await _orderRepository.AddAsync(order);
    }

    public async Task UpdateAsync( int id, UpdateOrderDto dto)
    {
        if (dto.OrderItems is null || dto.OrderItems.Count == 0)
            throw new ValidationException( "Order must contain at least one item.");

        Order? order =
            await _orderRepository.GetByIdWithDetailsAsync(id);

        if (order is null)
            throw new NotFoundException( "Order not found.");

        var result =
            await PrepareOrderItemsAsync(dto.OrderItems);

        order.OrderItems.Clear();

        foreach (OrderItem orderItem in result.OrderItems)
        {
            order.OrderItems.Add(orderItem);
        }

        order.TotalAmount = result.TotalAmount;

        await _orderRepository.UpdateAsync(order);
    }

    public async Task DeleteAsync(int id)
    {
        Order? order =
            await _orderRepository.GetByIdAsync(id);

        if (order is null)
            throw new NotFoundException("Order not found.");

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
            await _orderRepository
                .GetByIdWithDetailsAsync(id);

        if (order is null)
            return null;

        return _mapper.Map<OrderDetailDto>(order);
    }

    public async Task<List<OrderDto>> GetByDatesIntervalAsync( DateTime startDate,DateTime endDate)
    {
        if (startDate.Date > endDate.Date)
            throw new ValidationException("Start date cannot be greater than end date.");

        List<Order> orders =
            await _orderRepository
                .GetByDatesIntervalAsync( startDate, endDate);

        return _mapper.Map<List<OrderDto>>(orders);
    }

    public async Task<List<OrderDto>> GetByDateAsync(DateTime date)
    {
        List<Order> orders =
            await _orderRepository
                .GetByDateAsync(date);

        return _mapper.Map<List<OrderDto>>(orders);
    }

    public async Task<List<OrderDto>> GetByPriceIntervalAsync(decimal minPrice,decimal maxPrice)
    {
        if (minPrice < 0 || maxPrice < 0)
            throw new ValidationException("Price cannot be negative.");

        if (minPrice > maxPrice)
            throw new ValidationException("Minimum price cannot be greater than maximum price.");

        List<Order> orders =
            await _orderRepository
                .GetByPriceIntervalAsync(minPrice,maxPrice);

        return _mapper.Map<List<OrderDto>>(orders);
    }

    private async Task<(
        List<OrderItem> OrderItems,
        decimal TotalAmount)>
        PrepareOrderItemsAsync( List<CreateOrderItemDto> itemDtos)
    {
        List<OrderItem> orderItems = new();
        decimal totalAmount = 0;

        HashSet<int> menuItemIds = new();

        foreach (CreateOrderItemDto itemDto in itemDtos)
        {
            if (itemDto.MenuItemId <= 0)
                throw new ValidationException("Menu item Id must be greater than 0.");

            if (itemDto.Count <= 0)
                throw new ValidationException("Count must be greater than 0.");

            if (!menuItemIds.Add(itemDto.MenuItemId))
                throw new ValidationException(
                    $"Menu item with Id {itemDto.MenuItemId} " +
                    "was added more than once. " +
                    "Use Count instead.");

            MenuItem? menuItem =
                await _menuItemRepository
                    .GetByIdAsync(itemDto.MenuItemId);

            if (menuItem is null)
                throw new NotFoundException(
                    $"Menu item with Id " +
                    $"{itemDto.MenuItemId} not found.");

            OrderItem orderItem = new OrderItem
            {
                MenuItemId = menuItem.Id,
                Count = itemDto.Count
            };

            orderItems.Add(orderItem);

            totalAmount +=
                menuItem.Price * itemDto.Count;
        }

        return (orderItems, totalAmount);
    }
}
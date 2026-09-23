using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using RestaurantApp.Business.DTOs.Orders;
using RestaurantApp.Business.Exceptions;
using RestaurantApp.Business.MappingProfiles;
using RestaurantApp.Business.Services.Implementations;
using RestaurantApp.DataAccess.Repositories.Interfaces;
using RestaurantApp.Entity.Entities;
using RestaurantApp.Entity.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace TestProject1;

public class OrderServiceTests
{
    private readonly IMapper _mapper;

    public OrderServiceTests()
    {
        var config = new MapperConfiguration(
            cfg => cfg.AddProfile<AppMappingProfile>(),
            NullLoggerFactory.Instance
        );

        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task AddAsync_AddsOrder_CalculatesTotal()
    {
        var menuRepo = new FakeMenuItemRepository();
        menuRepo.Seed(new MenuItem { Id = 1, Name = "Burger", Price = 10m });

        var orderRepo = new FakeOrderRepository();

        var service = new OrderService(orderRepo, menuRepo, _mapper);

        var dto = new CreateOrderDto
        {
            OrderItems = new List<CreateOrderItemDto>
            {
                new CreateOrderItemDto { MenuItemId = 1, Count = 2 }
            }
        };

        await service.AddAsync(dto);

        Assert.Single(orderRepo.Orders);
        var added = orderRepo.Orders.First();
        Assert.Equal(20m, added.TotalAmount);
        Assert.Single(added.OrderItems);
        Assert.Equal(1, added.OrderItems.First().MenuItemId);
    }

    [Fact]
    public async Task AddAsync_ThrowsValidation_WhenNoItems()
    {
        var menuRepo = new FakeMenuItemRepository();
        var orderRepo = new FakeOrderRepository();
        var service = new OrderService(orderRepo, menuRepo, _mapper);

        await Assert.ThrowsAsync<ValidationException>(async () =>
            await service.AddAsync(new CreateOrderDto())
        );
    }

    [Fact]
    public async Task UpdateAsync_UpdatesExistingOrder()
    {
        var menuRepo = new FakeMenuItemRepository();
        menuRepo.Seed(new MenuItem { Id = 1, Name = "Burger", Price = 5m });
        menuRepo.Seed(new MenuItem { Id = 2, Name = "Fries", Price = 3m });

        var orderRepo = new FakeOrderRepository();
        var existing = new Order { Id = 1, Date = DateTime.Now };
        existing.OrderItems.Add(new OrderItem { MenuItemId = 1, Count = 1 });
        existing.TotalAmount = 5m;
        orderRepo.Orders.Add(existing);

        var service = new OrderService(orderRepo, menuRepo, _mapper);

        var dto = new UpdateOrderDto { OrderItems = new List<CreateOrderItemDto> {
            new CreateOrderItemDto { MenuItemId = 2, Count = 3 }
        }};

        await service.UpdateAsync(1, dto);

        var updated = orderRepo.Orders.First(o => o.Id == 1);
        Assert.Equal(9m, updated.TotalAmount);
        Assert.Single(updated.OrderItems);
        Assert.Equal(2, updated.OrderItems.First().MenuItemId);
    }

    [Fact]
    public async Task UpdateAsync_ThrowsNotFound_WhenMissing()
    {
        var menuRepo = new FakeMenuItemRepository();
        var orderRepo = new FakeOrderRepository();
        var service = new OrderService(orderRepo, menuRepo, _mapper);

        var dto = new UpdateOrderDto { OrderItems = new List<CreateOrderItemDto> { new CreateOrderItemDto { MenuItemId = 1, Count = 1 } } };

        await Assert.ThrowsAsync<NotFoundException>(async () =>
            await service.UpdateAsync(99, dto)
        );
    }

    [Fact]
    public async Task DeleteAsync_DeletesOrder()
    {
        var menuRepo = new FakeMenuItemRepository();
        var orderRepo = new FakeOrderRepository();
        var order = new Order { Id = 1 };
        orderRepo.Orders.Add(order);

        var service = new OrderService(orderRepo, menuRepo, _mapper);

        await service.DeleteAsync(1);

        Assert.Empty(orderRepo.Orders);
    }

    [Fact]
    public async Task DeleteAsync_ThrowsNotFound_WhenMissing()
    {
        var menuRepo = new FakeMenuItemRepository();
        var orderRepo = new FakeOrderRepository();
        var service = new OrderService(orderRepo, menuRepo, _mapper);

        await Assert.ThrowsAsync<NotFoundException>(async () =>
            await service.DeleteAsync(123)
        );
    }

    [Fact]
    public async Task GetAllAsync_ReturnsMappedDtos()
    {
        var menuRepo = new FakeMenuItemRepository();
        var orderRepo = new FakeOrderRepository();

        var o1 = new Order { Id = 1, Date = DateTime.Today, TotalAmount = 5m };
        o1.OrderItems.Add(new OrderItem { MenuItemId = 1, Count = 1, MenuItem = new MenuItem { Name = "A" } });
        orderRepo.Orders.Add(o1);

        var service = new OrderService(orderRepo, menuRepo, _mapper);
        var dtos = await service.GetAllAsync();

        Assert.Single(dtos);
        Assert.Equal(1, dtos.First().ItemCount);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNullWhenNotFound()
    {
        var menuRepo = new FakeMenuItemRepository();
        var orderRepo = new FakeOrderRepository();
        var service = new OrderService(orderRepo, menuRepo, _mapper);

        var dto = await service.GetByIdAsync(999);

        Assert.Null(dto);
    }

    [Fact]
    public async Task GetByDatesIntervalAsync_ThrowsWhenStartGreaterThanEnd()
    {
        var menuRepo = new FakeMenuItemRepository();
        var orderRepo = new FakeOrderRepository();
        var service = new OrderService(orderRepo, menuRepo, _mapper);

        await Assert.ThrowsAsync<ValidationException>(async () =>
            await service.GetByDatesIntervalAsync(DateTime.Today.AddDays(1), DateTime.Today)
        );
    }

    [Fact]
    public async Task GetByPriceIntervalAsync_Validation()
    {
        var menuRepo = new FakeMenuItemRepository();
        var orderRepo = new FakeOrderRepository();
        var service = new OrderService(orderRepo, menuRepo, _mapper);

        await Assert.ThrowsAsync<ValidationException>(async () =>
            await service.GetByPriceIntervalAsync(-1m, 5m)
        );

        await Assert.ThrowsAsync<ValidationException>(async () =>
            await service.GetByPriceIntervalAsync(10m, 5m)
        );
    }

    // --- Fake repositories used for tests ---
    private class FakeOrderRepository : IOrderRepository
    {
        public List<Order> Orders { get; } = new();

        public Task AddAsync(Order entity)
        {
            if (entity.Id == 0)
                entity.Id = Orders.Count + 1;
            Orders.Add(entity);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Order entity)
        {
            Orders.RemoveAll(o => o.Id == entity.Id);
            return Task.CompletedTask;
        }

        public Task<List<Order>> GetAllAsync() => Task.FromResult(Orders.ToList());

        public Task<List<Order>> GetAllWithDetailsAsync() => Task.FromResult(Orders.ToList());

        public Task<Order?> GetByIdAsync(int id) => Task.FromResult(Orders.FirstOrDefault(o => o.Id == id));

        public Task<Order?> GetByIdWithDetailsAsync(int id) => Task.FromResult(Orders.FirstOrDefault(o => o.Id == id));

        public Task<List<Order>> GetByDateAsync(DateTime date) => Task.FromResult(Orders.Where(o => o.Date.Date == date.Date).ToList());

        public Task<List<Order>> GetByDatesIntervalAsync(DateTime startDate, DateTime endDate) => Task.FromResult(Orders.Where(o => o.Date.Date >= startDate.Date && o.Date.Date <= endDate.Date).ToList());

        public Task<List<Order>> GetByPriceIntervalAsync(decimal minPrice, decimal maxPrice) => Task.FromResult(Orders.Where(o => o.TotalAmount >= minPrice && o.TotalAmount <= maxPrice).ToList());

        public Task UpdateAsync(Order entity)
        {
            var existing = Orders.FirstOrDefault(o => o.Id == entity.Id);
            if (existing != null)
            {
                existing.OrderItems = entity.OrderItems;
                existing.TotalAmount = entity.TotalAmount;
                existing.Date = entity.Date;
            }
            return Task.CompletedTask;
        }
    }

    private class FakeMenuItemRepository : IMenuItemRepository
    {
        private readonly List<MenuItem> _items = new();

        public void Seed(MenuItem item) => _items.Add(item);

        public Task AddAsync(MenuItem entity) => throw new NotImplementedException();

        public Task DeleteAsync(MenuItem entity) => throw new NotImplementedException();

        public Task<List<MenuItem>> GetAllAsync() => Task.FromResult(_items.ToList());

        public Task<MenuItem?> GetByIdAsync(int id) => Task.FromResult(_items.FirstOrDefault(x => x.Id == id));

        public Task<List<MenuItem>> GetByCategoryAsync(Category category) => Task.FromResult(_items.Where(x => x.Category == category).ToList());

        public Task<List<MenuItem>> GetByPriceIntervalAsync(decimal minPrice, decimal maxPrice) => Task.FromResult(_items.Where(x => x.Price >= minPrice && x.Price <= maxPrice).ToList());

        public Task<List<MenuItem>> SearchByNameAsync(string search) => Task.FromResult(_items.Where(x => x.Name.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList());

        public Task<bool> ExistsByNameAsync(string name, int? excludeId = null) => Task.FromResult(_items.Any(x => x.Name == name && (!excludeId.HasValue || x.Id != excludeId.Value)));

        public Task UpdateAsync(MenuItem entity) => throw new NotImplementedException();
    }
}

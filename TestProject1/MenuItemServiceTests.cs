using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using RestaurantApp.Business.DTOs.MenuItems;
using RestaurantApp.Business.Exceptions;
using RestaurantApp.Business.MappingProfiles;
using RestaurantApp.Business.Services.Implementations;
using RestaurantApp.DataAccess.Repositories.Interfaces;
using RestaurantApp.Entity.Entities;
using RestaurantApp.Entity.Entities.Enums;
using Xunit;

namespace TestProject1;

public class MenuItemServiceTests
{
    private readonly IMapper _mapper;

    public MenuItemServiceTests()
    {
        var config = new MapperConfiguration(
            cfg => cfg.AddProfile<AppMappingProfile>(),
            NullLoggerFactory.Instance
        );

        _mapper = config.CreateMapper();
    }


    [Fact]
    public async Task AddAsync_AddsMenuItemSuccessfully()
    {
        var repository = new FakeMenuItemRepository();

        var service = new MenuItemService(
            repository,
            _mapper);


        var dto = new CreateMenuItemDto
        {
            Name = "Burger",
            Price = 10m,
            Category = Category.FastFood
        };


        await service.AddAsync(dto);


        Assert.Single(repository.Items);

        var added = repository.Items.First();

        Assert.Equal("Burger", added.Name);
        Assert.Equal(10m, added.Price);
        Assert.Equal(Category.FastFood, added.Category);
    }


    [Fact]
    public async Task AddAsync_ThrowsValidation_WhenNameEmpty()
    {
        var repository = new FakeMenuItemRepository();

        var service = new MenuItemService(
            repository,
            _mapper);


        var dto = new CreateMenuItemDto
        {
            Name = "",
            Price = 10m,
            Category = Category.FastFood
        };


        await Assert.ThrowsAsync<ValidationException>(
            async () =>
            await service.AddAsync(dto));
    }


    [Fact]
    public async Task AddAsync_ThrowsValidation_WhenPriceInvalid()
    {
        var repository = new FakeMenuItemRepository();

        var service = new MenuItemService(
            repository,
            _mapper);


        var dto = new CreateMenuItemDto
        {
            Name = "Pizza",
            Price = 0,
            Category = Category.FastFood
        };


        await Assert.ThrowsAsync<ValidationException>(
            async () =>
            await service.AddAsync(dto));
    }


    [Fact]
    public async Task AddAsync_ThrowsAlreadyExists_WhenNameExists()
    {
        var repository = new FakeMenuItemRepository();

        repository.Seed(
            new MenuItem
            {
                Id = 1,
                Name = "Pizza",
                Price = 15,
                Category = Category.FastFood
            });


        var service = new MenuItemService(
            repository,
            _mapper);


        var dto = new CreateMenuItemDto
        {
            Name = "Pizza",
            Price = 20,
            Category = Category.FastFood
        };


        await Assert.ThrowsAsync<AlreadyExistsException>(
            async () =>
            await service.AddAsync(dto));
    }


    [Fact]
    public async Task UpdateAsync_UpdatesMenuItemSuccessfully()
    {
        var repository = new FakeMenuItemRepository();

        repository.Seed(
            new MenuItem
            {
                Id = 1,
                Name = "Burger",
                Price = 10,
                Category = Category.FastFood
            });


        var service = new MenuItemService(
            repository,
            _mapper);


        var dto = new UpdateMenuItemDto
        {
            Name = "Cheese Burger",
            Price = 15
        };


        await service.UpdateAsync(1, dto);


        var updated = repository.Items.First();


        Assert.Equal(
            "Cheese Burger",
            updated.Name);

        Assert.Equal(
            15,
            updated.Price);
    }


    [Fact]
    public async Task UpdateAsync_ThrowsNotFound_WhenIdMissing()
    {
        var repository = new FakeMenuItemRepository();

        var service = new MenuItemService(
            repository,
            _mapper);


        var dto = new UpdateMenuItemDto
        {
            Name = "Pizza",
            Price = 20
        };


        await Assert.ThrowsAsync<NotFoundException>(
            async () =>
            await service.UpdateAsync(99, dto));
    }


    [Fact]
    public async Task DeleteAsync_RemovesMenuItem()
    {
        var repository = new FakeMenuItemRepository();


        repository.Seed(
            new MenuItem
            {
                Id = 1,
                Name = "Pizza",
                Price = 15
            });


        var service = new MenuItemService(
            repository,
            _mapper);


        await service.DeleteAsync(1);


        Assert.Empty(repository.Items);
    }


    [Fact]
    public async Task DeleteAsync_ThrowsNotFound_WhenMissing()
    {
        var repository = new FakeMenuItemRepository();

        var service = new MenuItemService(
            repository,
            _mapper);


        await Assert.ThrowsAsync<NotFoundException>(
            async () =>
            await service.DeleteAsync(100));
    }


    [Fact]
    public async Task GetAllAsync_ReturnsMenuItemDtos()
    {
        var repository = new FakeMenuItemRepository();


        repository.Seed(
            new MenuItem
            {
                Id = 1,
                Name = "Pizza",
                Price = 15,
                Category = Category.FastFood
            });


        var service = new MenuItemService(
            repository,
            _mapper);


        var result =
            await service.GetAllAsync();


        Assert.Single(result);

        Assert.Equal(
            "Pizza",
            result.First().Name);
    }


    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        var repository = new FakeMenuItemRepository();

        var service = new MenuItemService(
            repository,
            _mapper);


        var result =
            await service.GetByIdAsync(50);


        Assert.Null(result);
    }


    [Fact]
    public async Task GetByCategoryAsync_ReturnsCorrectItems()
    {
        var repository = new FakeMenuItemRepository();


        repository.Seed(
            new MenuItem
            {
                Id = 1,
                Name = "Burger",
                Category = Category.FastFood
            });

        repository.Seed(
            new MenuItem
            {
                Id = 2,
                Name = "Soup",
                Category = Category.Soup
            });


        var service = new MenuItemService(
            repository,
            _mapper);


        var result =
            await service.GetByCategoryAsync(
                Category.FastFood);


        Assert.Single(result);

        Assert.Equal(
            "Burger",
            result.First().Name);
    }


    [Fact]
    public async Task GetByPriceIntervalAsync_ReturnsCorrectItems()
    {
        var repository = new FakeMenuItemRepository();


        repository.Seed(
            new MenuItem
            {
                Id = 1,
                Name = "Pizza",
                Price = 15
            });

        repository.Seed(
            new MenuItem
            {
                Id = 2,
                Name = "Coffee",
                Price = 3
            });


        var service = new MenuItemService(
            repository,
            _mapper);


        var result =
            await service.GetByPriceIntervalAsync(
                10,
                20);


        Assert.Single(result);

        Assert.Equal(
            "Pizza",
            result.First().Name);
    }


    [Fact]
    public async Task SearchAsync_ReturnsMatchingItems()
    {
        var repository = new FakeMenuItemRepository();


        repository.Seed(
            new MenuItem
            {
                Id = 1,
                Name = "Chicken Burger"
            });


        var service = new MenuItemService(
            repository,
            _mapper);


        var result =
            await service.SearchAsync("Burger");


        Assert.Single(result);
    }



    private class FakeMenuItemRepository : IMenuItemRepository
    {
        public List<MenuItem> Items { get; } = new();


        public void Seed(MenuItem item)
        {
            Items.Add(item);
        }


        public Task AddAsync(MenuItem entity)
        {
            if (entity.Id == 0)
                entity.Id = Items.Count + 1;

            Items.Add(entity);

            return Task.CompletedTask;
        }


        public Task DeleteAsync(MenuItem entity)
        {
            Items.Remove(entity);

            return Task.CompletedTask;
        }


        public Task<List<MenuItem>> GetAllAsync()
        {
            return Task.FromResult(
                Items.ToList());
        }


        public Task<MenuItem?> GetByIdAsync(int id)
        {
            return Task.FromResult(
                Items.FirstOrDefault(
                    x => x.Id == id));
        }


        public Task UpdateAsync(MenuItem entity)
        {
            var existing =
                Items.FirstOrDefault(
                    x => x.Id == entity.Id);


            if (existing != null)
            {
                existing.Name = entity.Name;
                existing.Price = entity.Price;
                existing.Category = entity.Category;
            }


            return Task.CompletedTask;
        }


        public Task<List<MenuItem>> GetByCategoryAsync(Category category)
        {
            return Task.FromResult(
                Items.Where(
                    x => x.Category == category)
                .ToList());
        }


        public Task<List<MenuItem>> GetByPriceIntervalAsync(
            decimal minPrice,
            decimal maxPrice)
        {
            return Task.FromResult(
                Items.Where(
                    x => x.Price >= minPrice &&
                         x.Price <= maxPrice)
                .ToList());
        }


        public Task<List<MenuItem>> SearchByNameAsync(string search)
        {
            return Task.FromResult(
                Items.Where(
                    x => x.Name.Contains(
                        search,
                        StringComparison.OrdinalIgnoreCase))
                .ToList());
        }


        public Task<bool> ExistsByNameAsync(
            string name,
            int? excludeId = null)
        {
            return Task.FromResult(
                Items.Any(
                    x => x.Name == name &&
                    (!excludeId.HasValue ||
                     x.Id != excludeId.Value)));
        }
    }
}

using RestaurantApp.Business.DTOs.MenuItems;
using RestaurantApp.Entity.Entities.Enums;

namespace RestaurantApp.Business.Services.Interfaces;

public interface IMenuItemService
{
    Task AddAsync(CreateMenuItemDto dto);

    Task UpdateAsync(int id, UpdateMenuItemDto dto);

    Task DeleteAsync(int id);

    Task<MenuItemDto?> GetByIdAsync(int id);

    Task<List<MenuItemDto>> GetAllAsync();

    Task<List<MenuItemDto>> GetByCategoryAsync(Category category);

    Task<List<MenuItemDto>> GetByPriceIntervalAsync( decimal minPrice, decimal maxPrice);

    Task<List<MenuItemDto>> SearchAsync(string search);
}
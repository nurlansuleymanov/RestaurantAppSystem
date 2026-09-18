using AutoMapper;
using RestaurantApp.Business.DTOs.MenuItems;
using RestaurantApp.Business.Exceptions;
using RestaurantApp.Business.Services.Interfaces;
using RestaurantApp.DataAccess.Repositories.Interfaces;
using RestaurantApp.Entity.Entities;
using RestaurantApp.Entity.Entities.Enums;

namespace RestaurantApp.Business.Services.Implementations;

public class MenuItemService : IMenuItemService
{
    private readonly IMenuItemRepository _menuItemRepository;
    private readonly IMapper _mapper;

    public MenuItemService( IMenuItemRepository menuItemRepository, IMapper mapper)
    {
        _menuItemRepository = menuItemRepository;
        _mapper = mapper;
    }

    public async Task AddAsync(CreateMenuItemDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ValidationException("Menu item name cannot be empty.");

        if (dto.Price <= 0)
            throw new ValidationException("Price must be greater than 0.");

        bool exists = await _menuItemRepository.ExistsByNameAsync(dto.Name);

        if (exists)
            throw new ValidationException("A menu item with this name already exists.");

        MenuItem menuItem = _mapper.Map<MenuItem>(dto);

        await _menuItemRepository.AddAsync(menuItem);
    }

    public async Task UpdateAsync(int id, UpdateMenuItemDto dto)
    {
        MenuItem? menuItem =await _menuItemRepository.GetByIdAsync(id);

        if (menuItem is null)
            throw new NotFoundException("Menu item not found.");

        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ValidationException("Menu item name cannot be empty.");

        if (dto.Price <= 0)
            throw new ValidationException("Price must be greater than 0.");

        bool exists = await _menuItemRepository
            .ExistsByNameAsync(dto.Name, id);

        if (exists)
            throw new AlreadyExistsException(
                "A menu item with this name already exists.");

        _mapper.Map(dto, menuItem);

        await _menuItemRepository.UpdateAsync(menuItem);
    }

    public async Task DeleteAsync(int id)
    {
        MenuItem? menuItem =await _menuItemRepository.GetByIdAsync(id);

        if (menuItem is null)
            throw new NotFoundException("Menu item not found.");

        await _menuItemRepository.DeleteAsync(menuItem);
    }

    public async Task<MenuItemDto?> GetByIdAsync(int id)
    {
        MenuItem? menuItem =
            await _menuItemRepository.GetByIdAsync(id);

        if (menuItem is null)
            return null;

        return _mapper.Map<MenuItemDto>(menuItem);
    }

    public async Task<List<MenuItemDto>> GetAllAsync()
    {
        List<MenuItem> menuItems =
            await _menuItemRepository.GetAllAsync();

        return _mapper.Map<List<MenuItemDto>>(menuItems);
    }

    public async Task<List<MenuItemDto>> GetByCategoryAsync(
        Category category)
    {
        List<MenuItem> menuItems =
            await _menuItemRepository
                .GetByCategoryAsync(category);

        return _mapper.Map<List<MenuItemDto>>(menuItems);
    }

    public async Task<List<MenuItemDto>> GetByPriceIntervalAsync( decimal minPrice, decimal maxPrice)
    {
        if (minPrice < 0 || maxPrice < 0)
            throw new Exception("Price cannot be negative.");

        if (minPrice > maxPrice)
            throw new ValidationException(
                "Minimum price cannot be greater than maximum price.");

        List<MenuItem> menuItems =
            await _menuItemRepository
                .GetByPriceIntervalAsync(minPrice, maxPrice);

        return _mapper.Map<List<MenuItemDto>>(menuItems);
    }

    public async Task<List<MenuItemDto>> SearchAsync( string search)
    {
        if (string.IsNullOrWhiteSpace(search))
            throw new ValidationException("Search value cannot be empty.");

        List<MenuItem> menuItems =
            await _menuItemRepository
                .SearchByNameAsync(search);

        return _mapper.Map<List<MenuItemDto>>(menuItems);
    }
}
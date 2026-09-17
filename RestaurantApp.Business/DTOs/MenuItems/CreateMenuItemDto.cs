using RestaurantApp.Entity.Entities.Enums;

namespace RestaurantApp.Business.DTOs.MenuItems;

public class CreateMenuItemDto
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public Category Category { get; set; }
}

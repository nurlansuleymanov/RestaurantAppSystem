namespace RestaurantApp.Business.DTOs.Orders;

public class OrderItemDto
{
    public int MenuItemId { get; set; }

    public string MenuItemName { get; set; } = string.Empty;

    public int Count { get; set; }
}
namespace RestaurantApp.Business.DTOs.Orders;

public class CreateOrderItemDto
{
    public int MenuItemId { get; set; }
    public int Count { get; set; }
}
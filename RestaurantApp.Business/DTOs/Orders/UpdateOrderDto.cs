namespace RestaurantApp.Business.DTOs.Orders;

public class UpdateOrderDto
{
    public List<CreateOrderItemDto> OrderItems { get; set; } = new();
}
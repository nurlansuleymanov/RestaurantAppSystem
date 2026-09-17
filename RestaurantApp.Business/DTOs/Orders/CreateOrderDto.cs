namespace RestaurantApp.Business.DTOs.Orders;

public class CreateOrderDto
{
    public List<CreateOrderItemDto> OrderItems { get; set; } = new();
}

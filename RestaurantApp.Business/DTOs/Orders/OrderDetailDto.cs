namespace RestaurantApp.Business.DTOs.Orders;

public class OrderDetailDto
{
    public int Id { get; set; }

    public decimal TotalAmount { get; set; }

    public int ItemCount { get; set; }

    public DateTime Date { get; set; }

    public List<OrderItemDto> OrderItems { get; set; } = new();
}

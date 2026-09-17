using RestaurantApp.Entity.Entities.Common;

namespace RestaurantApp.Entity.Entities;

public class Order : AuditEntity
{
    public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public decimal TotalAmount { get; set; }
    public DateTime Date { get; set; }
}

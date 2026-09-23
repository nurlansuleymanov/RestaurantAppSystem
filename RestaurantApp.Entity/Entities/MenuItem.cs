using RestaurantApp.Entity.Entities.Common;
using RestaurantApp.Entity.Entities.Enums;

namespace RestaurantApp.Entity.Entities;

public class MenuItem : AuditEntity
{
    public string Name { get; set; } = null!;

    public decimal Price { get; set; }

    public Category Category { get; set; }

    public List<OrderItem> OrderItems { get; set; }
        = new List<OrderItem>();
}
using RestaurantApp.Entity.Entities;
using RestaurantApp.Entity.Entities.Common;
using RestaurantApp.Entity.Entities.Enums;

public class MenuItem : AuditEntity
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public Category Category { get; set; }

    public OrderItem OrderItem { get; set; }
}

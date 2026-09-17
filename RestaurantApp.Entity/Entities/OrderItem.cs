using RestaurantApp.Entity.Entities.Common;

namespace RestaurantApp.Entity.Entities;

public class OrderItem : BaseEntity
{
    public int Count { get; set; }

    // MenuItem ile One-to-One
    public int MenuItemId { get; set; }
    public MenuItem MenuItem { get; set; }

    // Order ile One-to-Many
    public int OrderId { get; set; }
    public Order Order { get; set; }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantApp.Entity.Entities;

namespace RestaurantApp.DataAccess.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Count)
            .IsRequired();

        builder.HasOne(x => x.MenuItem)
            .WithOne(x => x.OrderItem)
            .HasForeignKey<OrderItem>(x => x.MenuItemId);
    }
}
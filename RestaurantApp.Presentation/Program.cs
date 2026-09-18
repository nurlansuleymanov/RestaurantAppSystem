using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RestaurantApp.Business.MappingProfiles;
using RestaurantApp.Business.Services.Implementations;
using RestaurantApp.Business.Services.Interfaces;
using RestaurantApp.DataAccess.Context;
using RestaurantApp.DataAccess.Repositories.Implementations;
using RestaurantApp.DataAccess.Repositories.Interfaces;


namespace RestaurantApp.Presentation
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ServiceCollection services = new ServiceCollection();
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer("Server=localhost;Database=RestaurantAppDb;Trusted_Connection=True;TrustServerCertificate=True;");
            });

            services.AddScoped<IMenuItemRepository, MenuItemRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IMenuItemService, MenuItemService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<AppMappingProfile>();
            });

            ServiceProvider serviceProvider = services.BuildServiceProvider();
            using IServiceScope scope = serviceProvider.CreateScope();

            IServiceProvider provider = scope.ServiceProvider;

            IMenuItemService menuItemService = provider.GetRequiredService<IMenuItemService>();
            IOrderService orderService = provider.GetRequiredService<IOrderService>();


            while (true)
            {
                Console.WriteLine("==== Restaurant App ====");
                Console.WriteLine("1. Menu operations");
                Console.WriteLine("2. Order operations");
                Console.WriteLine("3. Exit");

                Console.WriteLine("Choose an option:");
                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.WriteLine("Menu operations selected.");
                        break;
                    case "2":
                        Console.WriteLine("Order operations selected.");
                        break;

                    case "0":
                        return;

                    default:
                        Console.WriteLine("Wrong choice.");
                        break;
                }
            }
        
        }
    }
}

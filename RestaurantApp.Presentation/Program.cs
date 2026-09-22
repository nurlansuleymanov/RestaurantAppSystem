using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using RestaurantApp.Business.DTOs.MenuItems;
using RestaurantApp.Business.DTOs.Orders;
using RestaurantApp.Business.Exceptions;
using RestaurantApp.Business.MappingProfiles;
using RestaurantApp.Business.Services.Implementations;
using RestaurantApp.Business.Services.Interfaces;
using RestaurantApp.DataAccess.Context;
using RestaurantApp.DataAccess.Repositories.Implementations;
using RestaurantApp.DataAccess.Repositories.Interfaces;
using RestaurantApp.Entity.Entities.Enums;
using System.Globalization;
using System.Text;


// ======================================================
// CONSOLE SETTINGS
// ======================================================

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;


// ======================================================
// DATABASE CONTEXT
// ======================================================

using AppDbContext context = new AppDbContext();


// ======================================================
// REPOSITORIES
// ======================================================

IMenuItemRepository menuItemRepository =
    new MenuItemRepository(context);

IOrderRepository orderRepository =
    new OrderRepository(context);


// ======================================================
// AUTOMAPPER
// ======================================================

MapperConfiguration mapperConfiguration =
    new MapperConfiguration(
        cfg =>
        {
            cfg.AddProfile<AppMappingProfile>();
        },
        NullLoggerFactory.Instance
    );

IMapper mapper =
    mapperConfiguration.CreateMapper();


// ======================================================
// SERVICES
// ======================================================

IMenuItemService menuItemService =
    new MenuItemService(
        menuItemRepository,
        mapper);

IOrderService orderService =
    new OrderService(
        orderRepository,
        menuItemRepository,
        mapper);


// ======================================================
// PROGRAM START
// ======================================================

await MainMenuAsync();


// ======================================================
// MAIN MENU
// ======================================================

async Task MainMenuAsync()
{
    while (true)
    {
        Console.WriteLine();
        Console.WriteLine("======================================");
        Console.WriteLine("       RESTAURANT ORDER SYSTEM");
        Console.WriteLine("======================================");
        Console.WriteLine("1. Menu operations");
        Console.WriteLine("2. Order operations");
        Console.WriteLine("0. Exit");
        Console.WriteLine("======================================");

        int choice = ReadInt("Choose: ");

        switch (choice)
        {
            case 1:
                await MenuOperationsAsync();
                break;

            case 2:
                await OrderOperationsAsync();
                break;

            case 0:
                Console.WriteLine();
                Console.WriteLine("Program closed.");
                return;

            default:
                Console.WriteLine();
                Console.WriteLine("Invalid choice.");
                break;
        }
    }
}


// ======================================================
// MENU ITEM OPERATIONS
// ======================================================

async Task MenuOperationsAsync()
{
    while (true)
    {
        Console.WriteLine();
        Console.WriteLine("======================================");
        Console.WriteLine("          MENU OPERATIONS");
        Console.WriteLine("======================================");
        Console.WriteLine("1. Add new item");
        Console.WriteLine("2. Edit item");
        Console.WriteLine("3. Delete item");
        Console.WriteLine("4. Show all items");
        Console.WriteLine("5. Show items by category");
        Console.WriteLine("6. Show items by price interval");
        Console.WriteLine("7. Search items by name");
        Console.WriteLine("0. Back");
        Console.WriteLine("======================================");

        int choice = ReadInt("Choose: ");

        if (choice == 0)
            return;

        try
        {
            switch (choice)
            {
                // ======================================
                // ADD MENU ITEM
                // ======================================

                case 1:
                    {
                        Console.WriteLine();
                        Console.WriteLine("--- Add Menu Item ---");

                        string name =
                            ReadString("Name: ");

                        decimal price =
                            ReadDecimal("Price: ");

                        Category category =
                            ReadCategory();

                        CreateMenuItemDto dto =
                            new CreateMenuItemDto
                            {
                                Name = name,
                                Price = price,
                                Category = category
                            };

                        await menuItemService.AddAsync(dto);

                        Console.WriteLine();
                        Console.WriteLine(
                            "Menu item added successfully.");

                        break;
                    }


                // ======================================
                // UPDATE MENU ITEM
                // ======================================

                case 2:
                    {
                        Console.WriteLine();
                        Console.WriteLine("--- Edit Menu Item ---");

                        List<MenuItemDto> items =
                            await menuItemService.GetAllAsync();

                        PrintMenuItems(items);

                        if (items.Count == 0)
                            break;

                        int id =
                            ReadInt("Enter item Id: ");

                        string name =
                            ReadString("New name: ");

                        decimal price =
                            ReadDecimal("New price: ");

                        UpdateMenuItemDto dto =
                            new UpdateMenuItemDto
                            {
                                Name = name,
                                Price = price
                            };

                        await menuItemService
                            .UpdateAsync(id, dto);

                        Console.WriteLine();
                        Console.WriteLine(
                            "Menu item updated successfully.");

                        break;
                    }


                // ======================================
                // DELETE MENU ITEM
                // ======================================

                case 3:
                    {
                        Console.WriteLine();
                        Console.WriteLine("-- Delete Menu Item --");

                        List<MenuItemDto> items = await menuItemService.GetAllAsync();

                        PrintMenuItems(items);

                        if (items.Count == 0)
                            break;

                        int id =
                            ReadInt("Enter item Id: ");

                        await menuItemService.DeleteAsync(id);

                        Console.WriteLine();
                        Console.WriteLine(  "Menu item deleted successfully.");

                        break;
                    }


                // ======================================
                // GET ALL MENU ITEMS
                // ======================================

                case 4:
                    {
                        Console.WriteLine();
                        Console.WriteLine("--- All Menu Items ---");

                        List<MenuItemDto> items =
                            await menuItemService.GetAllAsync();

                        PrintMenuItems(items);

                        break;
                    }


                // ======================================
                // GET BY CATEGORY
                // ======================================

                case 5:
                    {
                        Console.WriteLine();
                        Console.WriteLine(
                            "--- Menu Items By Category ---");

                        Category category =
                            ReadCategory();

                        List<MenuItemDto> items =
                            await menuItemService
                                .GetByCategoryAsync(category);

                        PrintMenuItems(items);

                        break;
                    }


                // ======================================
                // GET BY PRICE INTERVAL
                // ======================================

                case 6:
                    {
                        Console.WriteLine();
                        Console.WriteLine(
                            "--- Menu Items By Price Interval ---");

                        decimal minPrice =
                            ReadDecimal("Minimum price: ");

                        decimal maxPrice =
                            ReadDecimal("Maximum price: ");

                        List<MenuItemDto> items =
                            await menuItemService
                                .GetByPriceIntervalAsync(
                                    minPrice,
                                    maxPrice);

                        PrintMenuItems(items);

                        break;
                    }


                // ======================================
                // SEARCH
                // ======================================

                case 7:
                    {
                        Console.WriteLine();
                        Console.WriteLine(
                            "--- Search Menu Item ---");

                        string search =
                            ReadString("Search text: ");

                        List<MenuItemDto> items =
                            await menuItemService
                                .SearchAsync(search);

                        PrintMenuItems(items);

                        break;
                    }


                default:
                    Console.WriteLine();
                    Console.WriteLine(
                        "Invalid choice.");
                    break;
            }
        }
        catch (ValidationException ex)
        {
            PrintError(ex.Message);
        }
        catch (AlreadyExistsException ex)
        {
            PrintError(ex.Message);
        }
        catch (NotFoundException ex)
        {
            PrintError(ex.Message);
        }
        catch (Exception ex)
        {
            PrintError(
                $"Unexpected error: {ex.Message}");
        }
    }
}


// ======================================================
// ORDER OPERATIONS
// ======================================================

async Task OrderOperationsAsync()
{
    while (true)
    {
        Console.WriteLine();
        Console.WriteLine("======================================");
        Console.WriteLine("          ORDER OPERATIONS");
        Console.WriteLine("======================================");
        Console.WriteLine("1. Add new order");
        Console.WriteLine("2. Cancel order");
        Console.WriteLine("3. Show all orders");
        Console.WriteLine("4. Show orders by date interval");
        Console.WriteLine("5. Show orders by price interval");
        Console.WriteLine("6. Show orders by date");
        Console.WriteLine("7. Show order by number");
        Console.WriteLine("0. Back");
        Console.WriteLine("======================================");

        int choice = ReadInt("Choose: ");

        if (choice == 0)
            return;

        try
        {
            switch (choice)
            {
                // ======================================
                // ADD ORDER
                // ======================================

                case 1:
                    {
                        Console.WriteLine();
                        Console.WriteLine("--- Add New Order ---");

                        List<MenuItemDto> menuItems =
                            await menuItemService.GetAllAsync();

                        if (menuItems.Count == 0)
                        {
                            Console.WriteLine();
                            Console.WriteLine(
                                "There are no menu items.");

                            break;
                        }

                        Console.WriteLine();
                        Console.WriteLine(
                            "Available menu items:");

                        PrintMenuItems(menuItems);

                        List<CreateOrderItemDto> orderItems =
                            new List<CreateOrderItemDto>();

                        while (true)
                        {
                            Console.WriteLine();

                            int menuItemId =
                                ReadInt(
                                    "Menu Item Id " +
                                    "(enter 0 to finish): ");

                            if (menuItemId == 0)
                                break;

                            int count =
                                ReadPositiveInt(
                                    "Count: ");

                            CreateOrderItemDto? existingItem =
                                orderItems.FirstOrDefault(
                                    x => x.MenuItemId == menuItemId);

                            if (existingItem is not null)
                            {
                                existingItem.Count += count;

                                Console.WriteLine(
                                    "This item was already added. " +
                                    "Count has been increased.");
                            }
                            else
                            {
                                orderItems.Add(
                                    new CreateOrderItemDto
                                    {
                                        MenuItemId = menuItemId,
                                        Count = count
                                    });
                            }
                        }

                        CreateOrderDto dto =
                            new CreateOrderDto
                            {
                                OrderItems = orderItems
                            };

                        await orderService.AddAsync(dto);

                        Console.WriteLine();
                        Console.WriteLine(
                            "Order added successfully.");

                        break;
                    }


                // ======================================
                // DELETE / CANCEL ORDER
                // ======================================

                case 2:
                    {
                        Console.WriteLine();
                        Console.WriteLine("--- Cancel Order ---");

                        int id =
                            ReadInt("Enter order number: ");

                        await orderService.DeleteAsync(id);

                        Console.WriteLine();
                        Console.WriteLine(
                            "Order cancelled successfully.");

                        break;
                    }


                // ======================================
                // GET ALL ORDERS
                // ======================================

                case 3:
                    {
                        Console.WriteLine();
                        Console.WriteLine("--- All Orders ---");

                        List<OrderDto> orders =
                            await orderService.GetAllAsync();

                        PrintOrders(orders);

                        break;
                    }


                // ======================================
                // GET BY DATE INTERVAL
                // ======================================

                case 4:
                    {
                        Console.WriteLine();
                        Console.WriteLine(
                            "--- Orders By Date Interval ---");

                        DateTime startDate =
                            ReadDate(
                                "Start date (dd.MM.yyyy): ");

                        DateTime endDate =
                            ReadDate(
                                "End date (dd.MM.yyyy): ");

                        // Includes the whole last day.
                        endDate =
                            endDate.Date
                                .AddDays(1)
                                .AddTicks(-1);

                        List<OrderDto> orders =
                            await orderService
                                .GetByDatesIntervalAsync(
                                    startDate,
                                    endDate);

                        PrintOrders(orders);

                        break;
                    }


                // ======================================
                // GET BY PRICE INTERVAL
                // ======================================

                case 5:
                    {
                        Console.WriteLine();
                        Console.WriteLine(
                            "--- Orders By Price Interval ---");

                        decimal minPrice =
                            ReadDecimal(
                                "Minimum amount: ");

                        decimal maxPrice =
                            ReadDecimal(
                                "Maximum amount: ");

                        List<OrderDto> orders =
                            await orderService
                                .GetByPriceIntervalAsync(
                                    minPrice,
                                    maxPrice);

                        PrintOrders(orders);

                        break;
                    }


                // ======================================
                // GET BY DATE
                // ======================================

                case 6:
                    {
                        Console.WriteLine();
                        Console.WriteLine(
                            "--- Orders By Date ---");

                        DateTime date =
                            ReadDate(
                                "Date (dd.MM.yyyy): ");

                        List<OrderDto> orders =
                            await orderService
                                .GetByDateAsync(date);

                        PrintOrders(orders);

                        break;
                    }


                // ======================================
                // GET BY ORDER NUMBER
                // ======================================

                case 7:
                    {
                        Console.WriteLine();
                        Console.WriteLine(
                            "--- Order Details ---");

                        int id =
                            ReadInt(
                                "Enter order number: ");

                        OrderDetailDto? order =
                            await orderService.GetByIdAsync(id);

                        if (order is null)
                        {
                            Console.WriteLine();
                            Console.WriteLine(
                                "Order not found.");

                            break;
                        }

                        PrintOrderDetail(order);

                        break;
                    }


                default:
                    Console.WriteLine();
                    Console.WriteLine(
                        "Invalid choice.");
                    break;
            }
        }
        catch (ValidationException ex)
        {
            PrintError(ex.Message);
        }
        catch (AlreadyExistsException ex)
        {
            PrintError(ex.Message);
        }
        catch (NotFoundException ex)
        {
            PrintError(ex.Message);
        }
        catch (Exception ex)
        {
            PrintError(
                $"Unexpected error: {ex.Message}");
        }
    }
}


// ======================================================
// PRINT MENU ITEMS
// ======================================================

void PrintMenuItems(List<MenuItemDto> items)
{
    Console.WriteLine();

    if (items.Count == 0)
    {
        Console.WriteLine(
            "No menu items found.");

        return;
    }

    Console.WriteLine(
        "---------------------------------------------------------------");

    Console.WriteLine(
        $"{"Id",-5}" +
        $"{"Name",-25}" +
        $"{"Category",-18}" +
        $"{"Price",-10}");

    Console.WriteLine(
        "---------------------------------------------------------------");

    foreach (MenuItemDto item in items)
    {
        Console.WriteLine(
            $"{item.Id,-5}" +
            $"{item.Name,-25}" +
            $"{item.Category,-18}" +
            $"{item.Price,-10:0.00}");
    }

    Console.WriteLine(
        "---------------------------------------------------------------");
}


// ======================================================
// PRINT ORDERS
// ======================================================

void PrintOrders(List<OrderDto> orders)
{
    Console.WriteLine();

    if (orders.Count == 0)
    {
        Console.WriteLine(
            "No orders found.");

        return;
    }

    Console.WriteLine(
        "-----------------------------------------------------------------------");

    Console.WriteLine(
        $"{"No",-8}" +
        $"{"Amount",-15}" +
        $"{"Item Count",-15}" +
        $"{"Date",-25}");

    Console.WriteLine(
        "-----------------------------------------------------------------------");

    foreach (OrderDto order in orders)
    {
        Console.WriteLine(
            $"{order.Id,-8}" +
            $"{order.TotalAmount,-15:0.00}" +
            $"{order.ItemCount,-15}" +
            $"{order.Date,-25:dd.MM.yyyy HH:mm}");
    }

    Console.WriteLine(
        "-----------------------------------------------------------------------");
}


// ======================================================
// PRINT ORDER DETAIL
// ======================================================

void PrintOrderDetail(OrderDetailDto order)
{
    Console.WriteLine();

    Console.WriteLine(
        "======================================");

    Console.WriteLine(
        $"Order number : {order.Id}");

    Console.WriteLine(
        $"Total amount : {order.TotalAmount:0.00}");

    Console.WriteLine(
        $"Item count   : {order.ItemCount}");

    Console.WriteLine(
        $"Date         : {order.Date:dd.MM.yyyy HH:mm}");

    Console.WriteLine(
        "======================================");

    Console.WriteLine();
    Console.WriteLine("Order items:");

    Console.WriteLine(
        "------------------------------------------------");

    Console.WriteLine(
        $"{"Item Id",-12}" +
        $"{"Name",-25}" +
        $"{"Count",-10}");

    Console.WriteLine(
        "------------------------------------------------");

    foreach (OrderItemDto item in order.OrderItems)
    {
        Console.WriteLine(
            $"{item.MenuItemId,-12}" +
            $"{item.MenuItemName,-25}" +
            $"{item.Count,-10}");
    }

    Console.WriteLine(
        "------------------------------------------------");
}


// ======================================================
// READ CATEGORY
// ======================================================

Category ReadCategory()
{
    while (true)
    {
        Console.WriteLine();
        Console.WriteLine("Categories:");

        foreach (Category category
                 in Enum.GetValues<Category>())
        {
            Console.WriteLine(
                $"{(int)category}. {category}");
        }

        Console.WriteLine();

        int categoryNumber =
            ReadInt("Choose category: ");

        if (Enum.IsDefined(
                typeof(Category),
                categoryNumber))
        {
            return (Category)categoryNumber;
        }

        Console.WriteLine();
        Console.WriteLine(
            "Invalid category.");
    }
}


// ======================================================
// READ INTEGER
// ======================================================

int ReadInt(string message)
{
    while (true)
    {
        Console.Write(message);

        string? input =
            Console.ReadLine();

        if (int.TryParse(
                input,
                out int value))
        {
            return value;
        }

        Console.WriteLine(
            "Please enter a valid number.");
    }
}


// ======================================================
// READ POSITIVE INTEGER
// ======================================================

int ReadPositiveInt(string message)
{
    while (true)
    {
        int value =
            ReadInt(message);

        if (value > 0)
            return value;

        Console.WriteLine(
            "Value must be greater than 0.");
    }
}


// ======================================================
// READ DECIMAL
// ======================================================

decimal ReadDecimal(string message)
{
    while (true)
    {
        Console.Write(message);

        string input =
            Console.ReadLine()?.Trim()
            ?? string.Empty;

        if (decimal.TryParse(
                input,
                NumberStyles.Number,
                CultureInfo.CurrentCulture,
                out decimal value))
        {
            return value;
        }

        string normalized =
            input.Replace(',', '.');

        if (decimal.TryParse(
                normalized,
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out value))
        {
            return value;
        }

        Console.WriteLine(
            "Please enter a valid amount.");
    }
}


// ======================================================
// READ STRING
// ======================================================

string ReadString(string message)
{
    while (true)
    {
        Console.Write(message);

        string value =
            Console.ReadLine()?.Trim()
            ?? string.Empty;

        if (!string.IsNullOrWhiteSpace(value))
            return value;

        Console.WriteLine( "Value cannot be empty.");
    }
}


// ======================================================
// READ DATE
// ======================================================

DateTime ReadDate(string message)
{
    while (true)
    {
        Console.Write(message);

        string input =
            Console.ReadLine()?.Trim()
            ?? string.Empty;

        bool result =
            DateTime.TryParseExact(
                input,
                "dd.MM.yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime date);

        if (result)
            return date;

        Console.WriteLine(
            "Invalid date. Example: 18.09.2026");
    }
}

// ======================================================
// PRINT ERROR
// ======================================================
void PrintError(string message)
{
    Console.WriteLine();
    Console.WriteLine($"Error: {message}");
}
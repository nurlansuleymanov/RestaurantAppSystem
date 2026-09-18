using AutoMapper;
using RestaurantApp.Business.DTOs.MenuItems;
using RestaurantApp.Business.DTOs.Orders;
using RestaurantApp.Entity.Entities;

namespace RestaurantApp.Business.MappingProfiles;

public class AppMappingProfile : Profile
{
    public AppMappingProfile()
    {
        // MenuItem mappings
        CreateMap<CreateMenuItemDto, MenuItem>();

        CreateMap<UpdateMenuItemDto, MenuItem>();

        CreateMap<MenuItem, MenuItemDto>();


        // OrderItem mappings
        CreateMap<CreateOrderItemDto, OrderItem>();

        CreateMap<OrderItem, OrderItemDto>()
            .ForMember(
                dest => dest.MenuItemName,
                opt => opt.MapFrom(src => src.MenuItem.Name)
            );


        // Order mappings
        CreateMap<CreateOrderDto, Order>()
            .ForMember(
                dest => dest.TotalAmount,
                opt => opt.Ignore()
            )
            .ForMember(
                dest => dest.Date, 
                opt => opt.Ignore()
            );

        CreateMap<Order, OrderDto>()
            .ForMember(
                dest => dest.ItemCount,
                opt => opt.MapFrom(
                    src => src.OrderItems.Sum(x => x.Count)
                ) 
            );

        CreateMap<Order, OrderDetailDto>()
            .ForMember(
                dest => dest.ItemCount,
                opt => opt.MapFrom(
                    src => src.OrderItems.Sum(x => x.Count)
                )
            );
    }
}

using AutoMapper;
using TicketAPI.Data.Models;
using TicketAPI.Data.Models.DTO;
using TicketAPI.Services.DTO;

namespace TicketAPI.Services.Helper.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Order, OrderDTO>();
        CreateMap<OrderItem, OrderItemDTO>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(o => o.Product.ProductId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(o => o.Product.Name));
        CreateMap<OrderItemPostDTO, OrderItem>()
            .ForMember(dest => dest.SinglePrice, opt => opt.MapFrom<SinglePriceResolver>());
        CreateMap<Order, OrderPreviewDTO>();
        CreateMap<Product, ProductDTO>();
        CreateMap<Product, ProductPreviewDTO>();
        CreateMap<ShoppingCartItemCreateDTO, ShoppingCartItem>();
        CreateMap<ShoppingCartItem, ShoppingCartItemCreateDTO>();
        CreateMap<ShoppingCartItem, ShoppingCartItemDTO>()
            .ForMember(dest => dest.Description, opt => opt.MapFrom(o => o.Product.Description))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(o => o.Product.Price))
            .ForMember(dest => dest.ImageName, opt => opt.MapFrom(o => o.Product.ImageName));
    }
}
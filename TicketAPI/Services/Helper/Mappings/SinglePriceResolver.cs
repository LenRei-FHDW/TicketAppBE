using AutoMapper;
using TicketAPI.Data;
using TicketAPI.Data.Models;
using TicketAPI.Data.Repositories;
using TicketAPI.Services.DTO;

namespace TicketAPI.Services.Helper.Mappings;

public class SinglePriceResolver(IRepository<Product, Guid> productRepository)
    : IValueResolver<OrderItemPostDTO, OrderItem, decimal>
{
    public decimal Resolve(OrderItemPostDTO source, OrderItem destination, decimal destMember,
        ResolutionContext context)
    {
        var product = productRepository.GetByIdAsync(source.ProductId).Result;
        return product.Price;
    }
}
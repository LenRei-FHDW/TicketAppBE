using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TicketAPI.Data;
using TicketAPI.Data.Exceptions;
using TicketAPI.Data.Models;
using TicketAPI.Data.Repositories;
using TicketAPI.Services.DTO;
using TicketAPI.Services.Helper;

namespace TicketAPI.Services.Scoped;

public class OrderService(OrderRepository _orderRepository, IRepository<OrderItem, Guid> _orderItemRepository, TicketApiDbContext _context, IMapper _mapper, UserManager<ApplicationUser> _userManager, EmailHelper _mailHelper){

    public async Task<IEnumerable<OrderPreviewDTO>> GetOrdersOfUsers(string email)
    {
       var orders = await _context.Orders
            .Where(o => o.ApplicationUser.Email == email).ToListAsync();
       return _mapper.Map<IEnumerable<OrderPreviewDTO>>(orders);
    }

    public async Task<OrderDTO> GetOrder(string userId, Guid id, bool isAdmin)
    {
        var order = await _orderRepository.GetByIdJoinOrderItems(id);
        if (order.ApplicationUserId == userId || isAdmin)
        {
            return _mapper.Map<OrderDTO>(order);
        }
        throw new ForbiddenException();
    }

    public async Task<OrderDTO> CreateNewOrder(string userId, IEnumerable<OrderItemPostDTO> orderItemsDtos)
    {
        var order = new Order
        {
            ApplicationUserId = userId
        };
        var orderItems = _mapper.Map<IEnumerable<OrderItem>>(orderItemsDtos).ToList();
        foreach (var item in orderItems)
        {
            item.Order = order;
        }
        await _orderItemRepository.AddRangeAsync(orderItems);
        var orderEntity = await _orderRepository.GetByIdAsynchLoadEager(order.OrderId);
        var orderDTO = _mapper.Map<OrderDTO>(orderEntity);
        return orderDTO;
    }
}

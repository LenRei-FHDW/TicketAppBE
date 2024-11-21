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

/// <summary>
/// Manages all actions of ordering.
/// </summary>
public class OrderService(OrderRepository _orderRepository, IRepository<OrderItem, Guid> _orderItemRepository, TicketApiDbContext _context, IMapper _mapper, ILogger<OrderService> _logger, UserManager<ApplicationUser> _userManager, EmailHelper _mailHelper)
{
    /// <summary>
    /// Collects all orders of the user with this email. 
    /// </summary>
    /// <param name="email">The email of the user.</param>
    /// <returns>OrderId, creation date and total price.</returns>
    public async Task<IEnumerable<OrderPreviewDTO>> GetOrdersOfUsers(string email)
    {
       var orders = await _context.Orders
            .Where(o => o.ApplicationUser.Email == email).ToListAsync();
       return _mapper.Map<IEnumerable<OrderPreviewDTO>>(orders);
    }

    /// <summary>
    /// Finds a specific order. This action is only allowed if the order is your own or if you are an admin.
    /// </summary>
    /// <param name="email">Mail of the user.</param>
    /// <param name="id">Id of the order.</param>
    /// <param name="isAdmin">Has the requester admin rights.</param>
    /// <returns>The order id, creation date, order items and total price.</returns>
    /// <exception cref="ForbiddenException">The user has no access rights for this order.</exception>
    public async Task<OrderDTO> GetOrder(string email, Guid id, bool isAdmin)
    {
        var order = await _orderRepository.GetByIdAsynchLoadEager(id);
        if (order.ApplicationUser.Email == email || isAdmin)
        {
            return _mapper.Map<OrderDTO>(order);
        }
        _logger.LogInformation("The user is not authorized to access this order.");
        throw new ForbiddenException();
    }

    /// <summary>
    /// Creates a new order for the user.
    /// </summary>
    /// <param name="email">Email of the user the order is created for.</param>
    /// <param name="orderItemsDtos">Details of the order item (productId, name, quantity, singleprice, totalprice).</param>
    /// <returns>The order id, creation date, order items and total price.</returns>
    public async Task<OrderDTO> CreateNewOrder(string email, IEnumerable<OrderItemPostDTO> orderItemsDtos)
    {
        var user = await _userManager.FindByEmailAsync(email);
        var order = new Order
        {
            ApplicationUser = user
        };
        var orderItems = _mapper.Map<IEnumerable<OrderItem>>(orderItemsDtos).ToList();
        foreach (var item in orderItems)
        {
            item.Order = order;
        }
        await _orderItemRepository.AddRangeAsync(orderItems);
        var orderEntity = await _orderRepository.GetByIdAsynchLoadEager(order.OrderId);
        var orderDTO = _mapper.Map<OrderDTO>(orderEntity);
        _logger.LogInformation("New order for email '{Email}' created.", email);
        return orderDTO;
    }
}

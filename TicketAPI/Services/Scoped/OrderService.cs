using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TicketAPI.Data;
using TicketAPI.Data.Exceptions;
using TicketAPI.Data.Models;
using TicketAPI.Data.Models.DTO;
using TicketAPI.Data.Repositories;
using TicketAPI.Services.DTO;
using TicketAPI.Services.Helper;

namespace TicketAPI.Services.Scoped;

/// <summary>
/// Manages all actions of ordering.
/// </summary>
public class OrderService(
    OrderRepository _orderRepository, 
    IRepository<OrderItem, Guid> _orderItemRepository, 
    TicketApiDbContext _context, 
    IMapper _mapper, 
    ILogger<OrderService> _logger, 
    UserManager<ApplicationUser> _userManager, 
    EmailHelper _mailHelper
    )
{
    /// <summary>
    /// Collects all orders of the user with this userId. 
    /// </summary>
    /// <param name="userId">The UserId of the user.</param>
    /// <returns>OrderId, creation date and total price.</returns>
    public async Task<IEnumerable<OrderPreviewDTO>> GetOrdersOfUsers(string userId)
    {
       var orders = await _context.Orders
            .Where(o => o.ApplicationUserId == userId)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .ToListAsync();
       return _mapper.Map<IEnumerable<OrderPreviewDTO>>(orders);
    }
    
    /// <summary>
    /// Finds a specific order. This action is only allowed if the order is your own or if you are an admin.
    /// </summary>
    /// <param name="userId">UserId of the user.</param>
    /// <param name="id">Id of the order.</param>
    /// <param name="isAdmin">Has the requester admin rights.</param>
    /// <returns>The order id, creation date, order items and total price.</returns>
    /// <exception cref="ForbiddenException">The user has no access rights for this order.</exception>
    public async Task<Order> GetOrderById(Guid orderId)
    {
        var order = await _orderRepository.GetByIdAsyncLoadEager(orderId);
        
        if (order == null)
        {
            _logger.LogInformation("Oder not found with ID: {orderId}", orderId);
            throw new KeyNotFoundException();
        }
        
        return order;
    }

    /// <summary>
    /// Finds a specific order. This action is only allowed if the order is your own or if you are an admin.
    /// </summary>
    /// <param name="userId">UserId of the user.</param>
    /// <param name="id">Id of the order.</param>
    /// <param name="isAdmin">Has the requester admin rights.</param>
    /// <returns>The order id, creation date, order items and total price.</returns>
    /// <exception cref="ForbiddenException">The user has no access rights for this order.</exception>
    public async Task<OrderDTO> GetOrder(string userId, Guid id, bool isAdmin)
    {
        var order = await _orderRepository.GetByIdAsyncLoadEager(id);
        if (order.ApplicationUserId == userId || isAdmin)
        {
            return _mapper.Map<OrderDTO>(order);
        }
        _logger.LogInformation("The user is not authorized to access this order.");
        throw new ForbiddenException();
    }

    /// <summary>
    /// Creates a new order for the user.
    /// </summary>
    /// <param name="userId">UserId of the user the order is created for.</param>
    /// <param name="shoppingCartItems">ShoppingCartItems of the user</param>
    /// <returns>The order id, creation date, order items and total price.</returns>
    public async Task<OrderDTO> CreateNewOrder(string userId, IEnumerable<ShoppingCartItem> shoppingCartItems)
    {
        var order = new Order
        {
            ApplicationUserId = userId
        };

        foreach (var shoppingCartItem in shoppingCartItems)
        {
            var orderItem = new OrderItem
            {
                ProductID = shoppingCartItem.ProductId,
                Quantity = shoppingCartItem.Quantity,
                SinglePrice = shoppingCartItem.Product.Price,
            };
            order.OrderItems.Add(orderItem);
        }
        
        var orderEntity = await _orderRepository.AddAsync(order);
        var orderDTO = _mapper.Map<OrderDTO>(orderEntity);
        _logger.LogInformation("New order for user '{userId}' created.", userId);
        return orderDTO;
    }
}

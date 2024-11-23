using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TicketAPI.Data.Models;
using TicketAPI.Services.DTO;
using TicketAPI.Services.Scoped;

namespace TicketAPI.Controllers;

/// <summary>
/// This controller manages all calls used for ordering articles.
/// </summary>
[Route("api/[controller]")]
public class OrderController(OrderService _orderService, UserManager<ApplicationUser> _userManager, ILogger<OrderController> _logger, IShoppingCartService _shoppingCartService) : ControllerBase
{
    /// <summary>
    /// Finds the current user and returns its orders.
    /// </summary>
    /// <returns>Returns the orders of the current user.</returns>
    [Authorize]
    [HttpGet("list")]
    public async Task<ActionResult<IEnumerable<OrderPreviewDTO>>> GetOrdersOfUser()
    {
        _logger.LogTrace("GetOrdersOfUser request received.");
        
        var userId = _userManager.GetUserId(User);
        if (userId == null)
        {
            _logger.LogWarning("UserId is null");
            return Unauthorized();
        }
        
        return Ok(await _orderService.GetOrdersOfUsers(userId));
    }

    /// <summary>
    /// Gets a specific order of the current user.
    /// </summary>
    /// <param name="id">Id of the order.</param>
    /// <returns>The specified order.</returns>
    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDTO>> GetOrder(Guid id)
    {
        _logger.LogTrace("GetOrder({Guid}) request received.", id);
        
        var userId = _userManager.GetUserId(User);
        if (userId == null)
        {
            _logger.LogWarning("UserId is null");
            return Unauthorized();
        }
        
        return Ok(await _orderService.GetOrder(userId, id, false));
    }

    /// <summary>
    /// Creates a new order.
    /// </summary>
    /// <param name="orderItems">The product id and the quantity of the items.</param>
    /// <returns>Order with his orderItems</returns>
    [Authorize]
    [HttpPost("create")]
    public async Task<ActionResult<OrderDTO>> CreateOrder()
    {
        _logger.LogTrace("CreateOrder request received.");
        
        var userId = _userManager.GetUserId(User);
        if (userId == null)
        {
            _logger.LogWarning("UserId is null");
            return Unauthorized();
        }

        var shoppingCartItems = await _shoppingCartService.GetModelItemsOfUser(userId);
        if (shoppingCartItems.Count() == 0)
        {
            _logger.LogWarning("No Product in ShoppingCart");
            return BadRequest("No Product in ShoppingCart");
        }

        var order = await _orderService.CreateNewOrder(userId, shoppingCartItems);

        await _shoppingCartService.RemoveAllFromCart(userId);
        
        return Ok(order);
    }
}
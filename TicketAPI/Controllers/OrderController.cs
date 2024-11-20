using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketAPI.Services.DTO;
using TicketAPI.Services.Scoped;

namespace TicketAPI.Controllers;

/// <summary>
/// This controller manages all calls used for ordering articles.
/// </summary>
[Route("api/[controller]")]
public class OrderController(OrderService _orderService, ILogger<OrderController> _logger) : ControllerBase
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
        var email = HttpContext.User?.FindFirst(ClaimTypes.Email)?.Value;
        return Ok(await _orderService.GetOrdersOfUsers(email));
    }

    /// <summary>
    /// Gets a specific order of the current user.
    /// </summary>
    /// <param name="id">Id of the order.</param>
    /// <returns>The specified order.</returns>
    [Authorize]
    [HttpGet("show/{id}")]
    public async Task<ActionResult<OrderDTO>> GetOrder(Guid id)
    {
        _logger.LogTrace("GetOrder({Guid}) request received.", id);
        var email = HttpContext.User?.FindFirst(ClaimTypes.Email)?.Value;
        return Ok(await _orderService.GetOrder(email, id, false));
    }

    /// <summary>
    /// Creates a new order.
    /// </summary>
    /// <param name="orderItems">The product id and the quantity of the items.</param>
    /// <returns></returns>
    [Authorize]
    [HttpPost("create")]
    public async Task<ActionResult<OrderDTO>> CreateOrder([FromBody] IEnumerable<OrderItemPostDTO> orderItems)
    {
        _logger.LogTrace("CreateOrder request received.");
        var email = HttpContext.User?.FindFirst(ClaimTypes.Email)?.Value;
        return Ok(await _orderService.CreateNewOrder(email, orderItems));
    }
}
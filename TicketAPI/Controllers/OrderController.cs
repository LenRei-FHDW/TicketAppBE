using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketAPI.Services.DTO;
using TicketAPI.Services.Scoped;

namespace TicketAPI.Controllers;

[Route("api/[controller]")]

public class OrderController(OrderService _orderService) : ControllerBase
{

    [Authorize]
    [HttpGet("list")]
    public async Task<ActionResult<IEnumerable<OrderPreviewDTO>>> GetOrdersOfUser()
    {
        var email = HttpContext.User?.FindFirst(ClaimTypes.Email)?.Value;
        return Ok(await _orderService.GetOrdersOfUsers(email));
    }

    [Authorize]
    [HttpGet("show/{id}")]
    public async Task<ActionResult<OrderDTO>> GetOrder(Guid id)
    {
        var email = HttpContext.User?.FindFirst(ClaimTypes.Email)?.Value;
        return Ok(await _orderService.GetOrder(email, id, false));
    }

    [Authorize]
    [HttpPost("create")]
    public async Task<ActionResult<OrderDTO>> CreateOrder([FromBody] IEnumerable<OrderItemPostDTO> orderItems)
    {
        var email = HttpContext.User?.FindFirst(ClaimTypes.Email)?.Value;
        return Ok(await _orderService.CreateNewOrder(email, orderItems));
    }
}
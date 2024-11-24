using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.BillingPortal;
using TicketAPI.Data.Models;
using TicketAPI.Services.DTO;
using TicketAPI.Services.Scoped;
using TicketAPI.Services.Stripe;

namespace TicketAPI.Controllers;

/// <summary>
/// This controller manages all calls used for ordering articles.
/// </summary>
[Route("api/[controller]")]
public class OrderController(
    OrderService orderService, 
    UserManager<ApplicationUser> userManager, 
    ILogger<OrderController> logger, 
    IShoppingCartService shoppingCartService, 
    IPaymentService playmentService,
    IConfiguration configuration
    ) : ControllerBase
{
    /// <summary>
    /// Finds the current user and returns its orders.
    /// </summary>
    /// <returns>Returns the orders of the current user.</returns>
    [Authorize]
    [HttpGet("list")]
    public async Task<ActionResult<IEnumerable<OrderPreviewDTO>>> GetOrdersOfUser()
    {
        logger.LogTrace("GetOrdersOfUser request received.");
        
        var userId = userManager.GetUserId(User);
        if (userId == null)
        {
            logger.LogWarning("UserId is null");
            return Unauthorized();
        }
        
        return Ok(await orderService.GetOrdersOfUsers(userId));
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
        logger.LogTrace("GetOrder({Guid}) request received.", id);
        
        var userId = userManager.GetUserId(User);
        if (userId == null)
        {
            logger.LogWarning("UserId is null");
            return Unauthorized();
        }
        
        return Ok(await orderService.GetOrder(userId, id, false));
    }

    /// <summary>
    /// Creates a new order.
    /// </summary>
    /// <param name="orderItems">The product id and the quantity of the items.</param>
    /// <returns>Order with his orderItems</returns>
    [Authorize]
    [HttpPost("create")]
    public async Task<ActionResult<Session>> CreateOrder()
    {
        logger.LogTrace("CreateOrder request received.");
        
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            logger.LogWarning("User is null");
            return Unauthorized();
        }

        var shoppingCartItems = await shoppingCartService.GetModelItemsOfUser(user.Id);
        if (shoppingCartItems.Count() == 0)
        {
            logger.LogWarning("No Product in ShoppingCart");
            return BadRequest("No Product in ShoppingCart");
        }

        var order = await orderService.CreateNewOrder(user.Id, shoppingCartItems);
        
        await shoppingCartService.RemoveAllFromCart(user.Id);
        
        var session = await playmentService.CreateCheckoutSession(order, user.Email);
        
        return Ok(session.Url);
    }
    
    
    [AllowAnonymous]
    [HttpPost("CompletOrder")]
    public async Task<IActionResult> PostCompletOrder()
    {
        var response = await playmentService.CompletOrder(Request);
        if(!response.Success)
        {
            return BadRequest(response.Message);
        }
        return Ok(response);
    }
    
    [AllowAnonymous]
    [HttpGet("CompletOrder")]
    public IActionResult GetCompletOrder()
    {
        var baseUrl = configuration.GetValue<string>("FrontEnd:BaseUrl");
        var callbackUrl = new UriBuilder(new Uri(baseUrl))
        {
            Path = configuration["FrontEnd:CallbackSuccess"]
        };
        return Redirect(callbackUrl.ToString());
    }
    
    [AllowAnonymous]
    [HttpGet("CanceledOrder/{orderId}")]
    public async Task<ActionResult> GetCanceledOrder(Guid orderId)
    {
        var newOrderId = await playmentService.CanceledOrder(orderId);
        
        
        var baseUrl = configuration.GetValue<string>("FrontEnd:BaseUrl");
        var callbackUrl = new UriBuilder(new Uri(baseUrl))
        {
            Path = configuration["FrontEnd:CallbackCanceled"]
        };
        
        var query  = HttpUtility.ParseQueryString(string.Empty);
        query["orderId"] = newOrderId.ToString();
        callbackUrl.Query = query.ToString();
        
        return Redirect(callbackUrl.ToString());
    }
}
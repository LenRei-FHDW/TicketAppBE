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
    IPaymentService paymentService,
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
    /// <param name="orderid">Id of the order.</param>
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
    /// Creates order session for stripe.
    /// </summary>
    /// <returns>Url to Stripe</returns>
    [Authorize]
    [HttpPost("create")]
    public async Task<ActionResult<string>> CreateOrder()
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
        
        var session = paymentService.CreateCheckoutSession(shoppingCartItems, user.Id, user.Email);
        
        return Ok(session.Url);
    }
    
    /// <summary>
    /// Webhook for Stripe if Order Completet
    /// </summary>
    /// <returns>Returns status for Stripe</returns>
    [AllowAnonymous]
    [HttpPost("CompletOrder")]
    public async Task<IActionResult> PostCompletOrder()
    {
        logger.LogInformation("Stripe Webhook call startet");
        var response = await paymentService.CompleteOrder(Request);
        if(!response.Success)
        {
            logger.LogWarning("Bad response from Stripe {message}", response.Message);
            return BadRequest(response.Message);
        }
        logger.LogInformation("Stripe Webhook call end");
        return Ok(response);
    }
    
    /// <summary>
    /// Redirect from Stripe to App back
    /// </summary>
    /// <returns>Redirect to App</returns>
    [AllowAnonymous]
    [HttpGet("CompletOrder")]
    public IActionResult GetCompletOrder()
    {
        logger.LogInformation("Redirect call to CallbackSuccess URL");
        var baseUrl = configuration.GetValue<string>("FrontEnd:BaseUrl");
        var callbackUrl = new UriBuilder(new Uri(baseUrl))
        {
            Path = configuration["FrontEnd:CallbackSuccess"]
        };
        return Redirect(callbackUrl.ToString());
    }
    
    /// <summary>
    /// Redirect from Stripe to App back if Canceled
    /// </summary>
    /// <returns>Redirect to App</returns>
    [AllowAnonymous]
    [HttpGet("CanceledOrder")]
    public ActionResult GetCanceledOrder()
    {
        logger.LogInformation("Redirect call to CallbackCanceled URL");
        var baseUrl = configuration.GetValue<string>("FrontEnd:BaseUrl");
        var callbackUrl = new UriBuilder(new Uri(baseUrl))
        {
            Path = configuration["FrontEnd:CallbackCanceled"]
        };
        
        return Redirect(callbackUrl.ToString());
    }
}
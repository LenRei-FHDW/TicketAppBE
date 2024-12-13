using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TicketAPI.Controllers.Helper;
using TicketAPI.Data.Models;
using TicketAPI.Data.Models.DTO;
using TicketAPI.Services.Scoped;

namespace TicketAPI.Controllers
{
    /// <summary>
    /// This controller manages all ShoppingCart api calls.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartController(
        UserManager<ApplicationUser> _userManager, 
        IShoppingCartService _shoppingCartService,
        ILogger<ProductController> _logger) 
        : ControllerBase
    {
        /// <summary>
        /// List all ShoppingCartItems
        /// </summary>
        /// <returns>IEnumerable of ShoppingCartItem of Authorize User</returns>
        // GET: api/Cart
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShoppingCartItemDTO>>> GetCartItems()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                _logger.LogWarning("UserId is null");
                return Unauthorized();
            }
            
            var cartItems = await _shoppingCartService.GetItemsOfUser(userId);
            return Ok(cartItems);
        }

        /// <summary>
        /// Add  Item to ShoppingCart
        /// </summary>
        /// /// <param name="cartItemDto">Item to add</param>
        /// <returns>ShoppingCartItemDTO of Authorize User</returns>
        // POST: api/Cart
        [HttpPost]
        public async Task<ActionResult<ShoppingCartItemDTO>> AddCartItem([FromBody] ShoppingCartItemCreateDTO cartItemDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("cartItemDto is invalid: {ModelState}", ModelState.ToString());
                return BadRequest(ModelState);
            }
            
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                _logger.LogWarning("UserId is null");
                return Unauthorized();
            }

            if (cartItemDto.Quantity == 0)
            {
                // LOG
                return BadRequest();
            }
            
            //var createDto = await _shoppingCartService.AddCartItem(cartItemDto, userId);
            await _shoppingCartService.AddCartItem(cartItemDto, userId);
            return Created();
        }

        /// <summary>
        /// Edit Item in ShoppingCart
        /// </summary>
        /// <param name="productId">ProductId</param>
        /// <param name="cartItemDto">Item to edit</param>
        /// <returns>NoContent</returns>
        // PUT: api/Cart/{productId}
        [ValidateModel]
        [HttpPut("{productId}")]
        public async Task<IActionResult> UpdateCartItem(Guid productId, [FromBody] ShoppingCartItemEditDTO cartItemDto)
        {
            ApplicationUser? user = await _userManager.GetUserAsync(User);
            if (!(user is null))
            {
                await _shoppingCartService.EditCartItem(productId, cartItemDto, user);
            }
            return NoContent();
        }
    }
}

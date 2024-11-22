using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TicketAPI.Data.Models;
using TicketAPI.Services.Scoped;

namespace TicketAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartController(
        UserManager<ApplicationUser> userManager, 
        IShoppingCartService shoppingCartService) 
        : ControllerBase
    {
        // GET: api/Cart
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShoppingCartItem>>> GetCartItems()
        {
            var userId = userManager.GetUserId(User);
            var cartItems = await shoppingCartService.GetItemsOfUser(userId);
            return Ok(cartItems);
        }

        // POST: api/Cart
        [HttpPost]
        public async Task<ActionResult<ShoppingCartItemDTO>> AddCartItem([FromBody] ShoppingCartItemCreateDTO cartItemDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = userManager.GetUserId(User);
            var createDto = await shoppingCartService.AddCartItem(cartItemDto, userId);
            return Created();
        }

        // PUT: api/Cart/{productId}
        [HttpPut("{productId}")]
        public async Task<IActionResult> UpdateCartItem(Guid productId, [FromBody] ShoppingCartItemEditDTO cartItemDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = userManager.GetUserId(User);
            shoppingCartService.EditCartItem(productId, cartItemDto, userId);
            return NoContent();
        }

        // DELETE: api/Cart/{productId}
        [HttpDelete("{productId}")]
        public async Task<IActionResult> RemoveCartItem(Guid productId)
        {
            var userId = userManager.GetUserId(User);
            await shoppingCartService.RemoveItemFromCart(userId, productId);
            return NoContent();
        }
    }
}

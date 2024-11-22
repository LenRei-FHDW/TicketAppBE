using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TicketAPI.Data.Models;
using TicketAPI.Data.Models.DTO;
using TicketAPI.Services.Scoped;

namespace TicketAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartController(
        UserManager<ApplicationUser> _userManager, 
        ShoppingCartService _shoppingCartService) 
        : ControllerBase
    {
        // GET: api/Cart
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShoppingCartItem>>> GetCartItems()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                //LOG
                return Unauthorized();
            }
            
            var cartItems = await _shoppingCartService.GetItemsOfUser(userId);
            return Ok(cartItems);
        }

        // POST: api/Cart
        [HttpPost]
        public async Task<ActionResult<ShoppingCartItemDTO>> AddCartItem([FromBody] ShoppingCartItemCreateDTO cartItemDto)
        {
            if (!ModelState.IsValid)
            {
                //LOG
                return BadRequest(ModelState);
            }
            
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                // LOG
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

        // PUT: api/Cart/{productId}
        [HttpPut("{productId}")]
        public async Task<IActionResult> UpdateCartItem(Guid productId, [FromBody] ShoppingCartItemEditDTO cartItemDto)
        {
            if (!ModelState.IsValid)
            {
                // LOG
                return BadRequest(ModelState);
            }
            
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                // LOG
                return Unauthorized();
            }

            if (cartItemDto.Quantity == 0)
            {
                await _shoppingCartService.RemoveItemFromCart(userId, productId);
                return NoContent();
            }
            
            _shoppingCartService.EditCartItem(productId, cartItemDto, userId);
            return NoContent();
        }
    }
}

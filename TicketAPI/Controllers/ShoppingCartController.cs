using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketAPI.Data;
using TicketAPI.Data.Models;
using TicketAPI.Services.DTO;

namespace TicketAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ShoppingCartController : ControllerBase
    {
        private readonly TicketApiDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ShoppingCartController(TicketApiDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        
        // GET: api/Cart
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShoppingCartItem>>> GetCartItems()
        {
            var userId = _userManager.GetUserId(User);
            var cartItems = await _context.ShoppingCartItems
                .Include(ci => ci.Product)
                .Where(ci => ci.ApplicationUserId == userId)
                .ToListAsync();

            return Ok(cartItems);
        }

        // POST: api/Cart
        [HttpPost]
        public async Task<ActionResult<ShoppingCartItem>> AddCartItem([FromBody] ShoppingCartItemCreateDTO cartItemDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = _userManager.GetUserId(User);
            var existingCartItem = await _context.ShoppingCartItems
                .FirstOrDefaultAsync(ci => ci.ApplicationUserId == userId && ci.ProductId == cartItemDto.ProductID);

            if (existingCartItem != null)
            {
                existingCartItem.Quantity += cartItemDto.Quantity;
                _context.Entry(existingCartItem).State = EntityState.Modified;
            }
            else
            {
                var newCartItem = new ShoppingCartItem
                {
                    ApplicationUserId = userId,
                    ProductId = cartItemDto.ProductID,
                    Quantity = cartItemDto.Quantity
                };

                _context.ShoppingCartItems.Add(newCartItem);
            }

            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCartItems), new { userId = userId }, cartItemDto);
        }

        // PUT: api/Cart/{productId}
        [HttpPut("{productId}")]
        public async Task<IActionResult> UpdateCartItem(Guid productId, [FromBody] SchoppingCartItemEditDTO cartItemDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = _userManager.GetUserId(User);
            var cartItem = await _context.ShoppingCartItems
                .FirstOrDefaultAsync(ci => ci.ApplicationUserId == userId && ci.ProductId == productId);

            if (cartItem == null)
            {
                return NotFound();
            }

            cartItem.Quantity = cartItemDto.Quantity;
            _context.Entry(cartItem).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Cart/{productId}
        [HttpDelete("{productId}")]
        public async Task<IActionResult> RemoveCartItem(Guid productId)
        {
            var userId = _userManager.GetUserId(User);
            var cartItem = await _context.ShoppingCartItems
                .FirstOrDefaultAsync(ci => ci.ApplicationUserId == userId && ci.ProductId == productId);

            if (cartItem == null)
            {
                return NotFound();
            }

            _context.ShoppingCartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

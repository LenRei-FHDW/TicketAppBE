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
    public class AddressController : ControllerBase
    {
        private readonly TicketApiDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AddressController(TicketApiDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        
        // GET: api/Address
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Address>>> GetAddresses()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }
            
            return await _context.Addresses.Where(a => a.ApplicationUserId == userId).ToListAsync();
        }

        // GET: api/Address/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Address>> GetAddress(Guid id)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }
            
            var address = await _context.Addresses
                .Where(a => a.ApplicationUserId == userId)
                .FirstOrDefaultAsync(a => a.AddressId == id);
            
            if (address == null)
            {
                return NotFound();
            }
            
            return address;
        }

        // POST: api/Address
        [HttpPost]
        public async Task<ActionResult<Address>> CreateAddress(AddressCreateDTO addressDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(addressDTO);
            }
            
            var userId = _userManager.GetUserId(User);

            var newAddress = new Address
            {
                ApplicationUserId = userId,
                Street = addressDTO.Street,
                City = addressDTO.City,
                State = addressDTO.State,
                Zip = addressDTO.Zip,
                Country = addressDTO.Country,
            };
            
            _context.Addresses.Add(newAddress);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAddress), new { id = newAddress.AddressId }, newAddress);
        }

        // PUT: api/Address/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAddress(Guid id, [FromBody] AddressEditDTO addressDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(addressDTO);
            }
            
            var address = await _context.Addresses.FindAsync(id);
            if (address == null)
            {
                return NotFound();
            }
            
            address.Street = addressDTO.Street;
            address.City = addressDTO.City;
            address.State = addressDTO.State;
            address.Zip = addressDTO.Zip;
            address.Country = addressDTO.Country;

            _context.Entry(address).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AddressExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Address/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddress(Guid id)
        {
            var address = await _context.Addresses.FindAsync(id);
            if (address == null)
            {
                return NotFound();
            }

            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AddressExists(Guid id)
        {
            return _context.Addresses.Any(e => e.AddressId == id);
        }
    }
}

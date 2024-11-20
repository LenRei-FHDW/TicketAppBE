using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketAPI.Data;
using TicketAPI.Data.Models;
using TicketAPI.Data.Models.DTO;

namespace TicketAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        
        private readonly TicketApiDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(TicketApiDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        [HttpGet]
        [Authorize]
        public ActionResult<UserDataResultDTO> GetUserData()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return Unauthorized();
            
            var user = _context.Users
                .Include(u => u.Addresse)
                .FirstOrDefault(u => u.Id == userId);
            
            if(user == null)
                return NotFound();

            var userDataResultDTO = new UserDataResultDTO()
            {
                ApplicationUserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Street = user.Addresse?.Street ?? string.Empty,
                City = user.Addresse?.City ?? string.Empty,
                Zip = user.Addresse?.Zip ?? string.Empty,
            };
            
            return userDataResultDTO;
        }

        [HttpPut]
        [Authorize]
        public async Task<ActionResult<UserDataEditDTO>> PutUserData([FromBody] UserDataEditDTO userDataDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return Unauthorized();

            if (userDataDTO.ApplicationUserId != userId)
                return BadRequest();
            
            var user = _context.Users
                .Include(u => u.Addresse)
                .FirstOrDefault(u => u.Id == userId);
            
            if(user == null)
                return NotFound();
            
            user.FirstName = userDataDTO.FirstName;
            user.LastName = userDataDTO.LastName;
            
            if (user.Addresse == null)
            {
                user.Addresse = new Address
                {
                    ApplicationUserId = userId,
                    Street = userDataDTO.Street,
                    City = userDataDTO.City,
                    Zip = userDataDTO.Zip
                };
                
                _context.Entry(user.Addresse).State = EntityState.Added;
            }
            else
            {
                user.Addresse.Street = userDataDTO.Street;
                user.Addresse.City = userDataDTO.City;
                user.Addresse.Zip = userDataDTO.Zip;
                _context.Entry(user.Addresse).State = EntityState.Modified;
            }
            
            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                if(!_context.Users.Any(u => u.Id == userId))
                    return NotFound();
                throw;
            }
            
            return Ok(userDataDTO);
            
        }
    }
}

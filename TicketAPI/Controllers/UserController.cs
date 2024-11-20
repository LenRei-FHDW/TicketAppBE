using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketAPI.Data;
using TicketAPI.Data.Models;
using TicketAPI.Data.Models.DTO;
using TicketAPI.Services.Scoped;

namespace TicketAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserService _userService;

        public UserController(UserManager<ApplicationUser> userManager, IUserService userService)
        {
            _userManager = userManager;
            _userService = userService;
        }


        [HttpGet]
        [Authorize]
        public async Task<ActionResult<UserDataResultDTO>> GetUserData()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return Unauthorized();

            var userData = await _userService.GetUserDataAsync(userId);
            if (userData == null)
                return NotFound();

            return Ok(userData);
        }

        [HttpPut]
        [Authorize]
        public async Task<ActionResult<UserDataEditDTO>> PutUserData([FromBody] UserDataEditDTO userDataDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return Unauthorized();

            if (userDataDTO.ApplicationUserId != userId)
                return BadRequest();

            var updatedUserData = await _userService.UpdateUserDataAsync(userId, userDataDTO);
            if (updatedUserData == null)
                return NotFound();

            return Ok(updatedUserData);
        }
    }
}

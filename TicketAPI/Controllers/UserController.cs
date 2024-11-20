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
    /// <summary>
    /// This controller manages all user api calls.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(UserManager<ApplicationUser> _userManager, IUserService _userService) : ControllerBase
    {
        /// <summary>
        /// Response to authorized user with his data
        /// </summary>
        /// <returns>userData from the User</returns>
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

        /// <summary>
        /// Response to authorized user with changes userData
        /// </summary>
        /// <param name="userDataDTO">ApplicationUserId, FirstName, LastName, Street, City, Zip for updates data</param>
        /// <returns>Updates UserData</returns>
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

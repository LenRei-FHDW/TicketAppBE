using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketAPI.Services.Scoped;
using TicketAPI.Services.DTO;

namespace TicketAPI.Controllers
{
    /// <summary>
    /// This controller manages all authentification api calls.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(AuthService _authService, ILogger<AuthController> _logger) : ControllerBase
    {
        /// <summary>
        /// Response to a login call.
        /// </summary>
        /// <param name="model">Email and password of the login.</param>
        /// <returns>A token if successfull.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModelDTO model)
        {
            _logger.LogTrace("Login request received.");
            var result = await _authService.LoginAsync(model);

            if (result == null)
            {
                _logger.LogWarning("Invalid login attempt.");
                return Unauthorized("Invalid login attempt.");
            }

            if (!result.IsEmailConfirmed)
            {
                _logger.LogWarning("Unconfirmed email '{Email}'", model.Email);
                return BadRequest("Email not confirmed. Please check your email to confirm your account.");
            }

            return Ok(new { token = result.Token });
        }

        /// <summary>
        /// Response to a register call.
        /// </summary>
        /// <param name="model">Full name, email and password used to register.</param>
        /// <returns>The result of the request.</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModelDTO model)
        {
            _logger.LogTrace("Register request received.");
            RegisterResultDTO result = await _authService.Register(model);
            if (!result.SamePassword)
            {
                _logger.LogWarning("Password '{Password}' and repeat password '{RepeatPassword}' do not match.", model.Password, model.RepeatPassword);
                return BadRequest("Passwords do not match.");
            }

            if (result.Succeeded)
            {
                return Ok(
                    $"\"Registration successful. Please check your email to confirm your account.\"");
            }
            return BadRequest();
        }
        
		[Authorize(Roles = "Admin")]
        [HttpPost("delete-user-admin")]
        public async Task<IActionResult> DeleteUser()
        {
            _logger.LogTrace("DeleteUser request received.");
            var email = HttpContext.User?.FindFirst("Email")?.Value;
            await _authService.DeleteUser(new ResendConfirmationEmailModelDTO(){Email = email});
            return NoContent();
        }

        /// <summary>
        /// Removes a user.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("delete-user")]
        public async Task<IActionResult> DeleteUser([FromBody] ResendConfirmationEmailModelDTO model)
        {
            // #TODO Secure the deletion
            await _authService.DeleteUser(model);
            return NoContent();
        }
    }
}
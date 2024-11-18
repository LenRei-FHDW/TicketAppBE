using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketAPI.Services.Scoped;
using TicketAPI.Services.DTO;

namespace TicketAPI.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        
        public AuthController( AuthService authService)
        {
            _authService = authService;
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModelDTO model)
        {
            var result = await _authService.LoginAsync(model);

            if (result == null)
            {
                return Unauthorized("Invalid login attempt.");
            }

            if (!result.IsEmailConfirmed)
            {
                return BadRequest("Email not confirmed. Please check your email to confirm your account.");
            }

            return Ok(new { token = result.Token });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModelDTO model)
        {
            RegisterResultDTO result = await _authService.Register(model);
            if (!result.SamePassword)
            {
                return BadRequest("Passwords do not match.");
            }

            if (result.Succeeded)
            {
                return Ok(
                    $"\"Registration successful. Please check your email to confirm your account.\"");
            }
            return BadRequest();
        }
        
        [HttpPost("delete-user-admin")]
        public async Task<IActionResult> DeleteUser()
        {
            var email = HttpContext.User?.FindFirst("Email")?.Value;
            await _authService.DeleteUser(new ResendConfirmationEmailModelDTO(){Email = email});
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("delete-user")]
        public async Task<IActionResult> DeleteUser([FromBody] ResendConfirmationEmailModelDTO model)
        {
            await _authService.DeleteUser(model);
            return NoContent();
        }
    }
}
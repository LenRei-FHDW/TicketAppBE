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
    public class AuthController(AuthService _authService, EmailService _emailService, PasswordService _passwordService, ILogger<AuthController> _logger) : ControllerBase
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

        /// <summary>
        /// Called to confirms that the email is really owned by the caller.
        /// </summary>
        /// <param name="userId">Which user wants to confirm the mail.</param>
        /// <param name="code">Code used to confirm the user.</param>
        /// <returns>A response based on success or failure.</returns>
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            _logger.LogTrace("ConfirmEmail request received.");
            var result = await _emailService.ConfirmEmail(userId, code);
            if (result.EmailConfirmed)
                return Ok("Email confirmed successfully.");
            if (!result.UserExists)
                return NotFound("User not found.");
            if (!result.CompleteRequest)
                return BadRequest("User ID and confirmation code are required.");
            if (result.EmailConfirmed)
                return Ok("Email confirmed successfully.");
            return BadRequest("Email confirmation failed.");
        }

        /// <summary>
        /// Resents the confirmation mail on call.
        /// </summary>
        /// <param name="model">Which mail should be resent</param>
        /// <returns>A response based on success or failure.</returns>
        [HttpPost("resend-confirmation-email")]
        public async Task<IActionResult> ResendConfirmationEmail([FromBody] ResendConfirmationEmailModelDTO model)
        {
            _logger.LogTrace("ResendConfirmationEmail request received.");
            ConfirmEmailResultDTO result = await _emailService.ResendConfirmationEmail(model);
            if (!result.UserExists)
            {
                return NotFound("User not found.");
            }

            if (result.EmailConfirmed)
            {
                return BadRequest("Email is already confirmed.");
            }

            return Ok("Confirmation email sent.");
        }

        /// <summary>
        /// Sends a mail with a reset link.
        /// </summary>
        /// <param name="model">Which account should be sent a forgot password mail </param>
        /// <returns></returns>
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordModelDTO model)
        {
            _logger.LogTrace("ForgotPassword request received.");
            return Ok(await _passwordService.ForgotPassword(model));
        }

        /// <summary>
        /// Resets the password.
        /// </summary>
        /// <param name="model">The email, auth token and new password.</param>
        /// <returns>A response based on success or failure.</returns>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModelDTO model)
        {
            _logger.LogTrace("ResetPassword request received.");
            bool result = await _passwordService.ResetPassword(model);
            return result ? Ok() : BadRequest();
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
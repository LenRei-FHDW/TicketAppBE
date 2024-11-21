using Microsoft.AspNetCore.Mvc;
using TicketAPI.Services;
using TicketAPI.Services.DTO;
using TicketAPI.Services.Scoped;

namespace TicketAPI.Controllers;

/// <summary>
/// This controller manages all calls for password interaction.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PasswordController(PasswordService _passwordService, ILogger<PasswordController> _logger) : ControllerBase
{
    /// <summary>
    /// Sends a mail with a reset link.
    /// </summary>
    /// <param name="model"></param>
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
}
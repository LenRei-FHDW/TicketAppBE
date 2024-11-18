using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketAPI.Services;
using TicketAPI.Services.DTO;
using TicketAPI.Services.Scoped;

namespace TicketAPI.Controllers;

/// <summary>
/// This controller manages all email calls.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class EmailController: ControllerBase
{
    private readonly EmailService _emailService;

    public EmailController(EmailService emailService)
    {
        _emailService = emailService;
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
}
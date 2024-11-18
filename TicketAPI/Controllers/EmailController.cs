using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketAPI.Services;
using TicketAPI.Services.DTO;
using TicketAPI.Services.Scoped;

namespace TicketAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmailController: ControllerBase
{
    private readonly EmailService _emailService;

    public EmailController(EmailService emailService)
    {
        _emailService = emailService;
    }

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
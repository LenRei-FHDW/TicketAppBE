using Microsoft.AspNetCore.Mvc;
using TicketAPI.Services;
using TicketAPI.Services.DTO;
using TicketAPI.Services.Scoped;

namespace TicketAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PasswordController : ControllerBase
{

    private readonly PasswordService _passwordService;

    public PasswordController(PasswordService passwordService)
    {
        _passwordService = passwordService;
    }


    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordModelDTO model)
    {
        return Ok(await _passwordService.ForgotPassword(model));
    }
    
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModelDTO model)
    {
        bool result = await _passwordService.ResetPassword(model);
        return result ? Ok() : BadRequest();
    }
}
using Microsoft.AspNetCore.Identity;
using TicketAPI.Data.Models;
using TicketAPI.Services.DTO;
using TicketAPI.Services.Helper;

namespace TicketAPI.Services.Scoped;

public class EmailService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly EmailHelper _emailHelper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public EmailService(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor, EmailHelper emailHelper)
    {
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
        _emailHelper = emailHelper;
    }

    public async Task<ConfirmEmailResultDTO> ConfirmEmail(string userId, string code)
    {
        if (userId == null || code == null)
            return new ConfirmEmailResultDTO(false, false, false);

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return new ConfirmEmailResultDTO(true, false, false);

        var result = await _userManager.ConfirmEmailAsync(user, code);
        if (result.Succeeded)
            return new ConfirmEmailResultDTO(true, true, true);

        return new ConfirmEmailResultDTO(true, true, false);
    }

    public async Task<ConfirmEmailResultDTO> ResendConfirmationEmail(ResendConfirmationEmailModelDTO model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return new ConfirmEmailResultDTO(false, false);
        }

        if (user.EmailConfirmed)
        {
            return new ConfirmEmailResultDTO(true, true);
        }
        
        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        HttpContext? context = _httpContextAccessor.HttpContext;
        _emailHelper.GenerateVerificationEmail(code, context, user.Email, user.Id);
        return new ConfirmEmailResultDTO(true, false);
    }
}
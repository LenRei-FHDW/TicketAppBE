using Microsoft.AspNetCore.Identity;
using TicketAPI.Data.Models;
using TicketAPI.Services.DTO;
using TicketAPI.Services.Transient;

namespace TicketAPI.Services.Scoped;

public class PasswordService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly EmailHelper _emailHelper;
    private readonly IHttpContextAccessor _httpContextAccessor;


    public PasswordService(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor, EmailHelper emailHelper)
    {
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
        _emailHelper = emailHelper;
    }

    public async Task<string> ForgotPassword(ForgotPasswordModelDTO model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
        {
            return "If your email is registered, you will receive a password reset link.";
        }
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        _emailHelper.GenerateResetEmail(token, user.Email, user.Id);
        return "Password reset email sent. Please check your email.";
    }

    public async Task<bool> ResetPassword(ResetPasswordModelDTO model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            // Wenn der Benutzer nicht existiert, geben wir eine Erfolgsmeldung zurück, um die Privatsphäre zu schützen.
            return true;
        }

        var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
        return result.Succeeded;
    }
}
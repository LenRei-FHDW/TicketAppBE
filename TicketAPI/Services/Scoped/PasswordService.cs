using Microsoft.AspNetCore.Identity;
using TicketAPI.Data.Models;
using TicketAPI.Services.DTO;
using TicketAPI.Services.Helper;

namespace TicketAPI.Services.Scoped;

/// <summary>
/// Manages password interactions.
/// </summary>
public class PasswordService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly EmailHelper _emailHelper;


    public PasswordService(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor, EmailHelper emailHelper)
    {
        _userManager = userManager;
        _emailHelper = emailHelper;
    }

    /// <summary>
    /// Sends a password reset mail if there is a matching user.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Resets the password if the token matches.
    /// </summary>
    /// <param name="model">The user email, token and new password.</param>
    /// <returns>The success of the action.</returns>
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
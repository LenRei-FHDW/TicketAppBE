using Microsoft.AspNetCore.Identity;
using TicketAPI.Data.Models;
using TicketAPI.Services.DTO;
using TicketAPI.Services.Helper;

namespace TicketAPI.Services.Scoped;

/// <summary>
/// Sends emails to the target.
/// </summary>
public class EmailService(UserManager<ApplicationUser> _userManager, IHttpContextAccessor _httpContextAccessor, ILogger<EmailService> _logger, EmailHelper _emailHelper)
{
    /// <summary>
    /// Checks if the used code matches the user.
    /// </summary>
    /// <param name="userId">The user that requests the confirmation.</param>
    /// <param name="code">The confirmation code to check.</param>
    /// <returns>That the user exists, the request completed and wether it succeeded.</returns>
    public async Task<ConfirmEmailResultDTO> ConfirmEmail(string userId, string code)
    {
        if (userId == null || code == null)
        {
            _logger.LogWarning("UserId or code received was empty.");
            return new ConfirmEmailResultDTO(false, false, false);
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            _logger.LogWarning("User with id '{UserId}' not found.", userId);
            return new ConfirmEmailResultDTO(true, false, false);
        }

        var result = await _userManager.ConfirmEmailAsync(user, code);
        if (result.Succeeded)
        {
            _logger.LogInformation("Confirmation mail has been sent to '{Email}'.", user.Email);
            return new ConfirmEmailResultDTO(true, true, true);
        }

        _logger.LogError("Failed to send confirmation mail.");
        return new ConfirmEmailResultDTO(true, true, false);
    }

    /// <summary>
    /// Resends the email with a confirmation token.
    /// </summary>
    /// <param name="model">The target email.</param>
    /// <returns>That the user exists and wether the email was confirmed.</returns>
    public async Task<ConfirmEmailResultDTO> ResendConfirmationEmail(ResendConfirmationEmailModelDTO model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            _logger.LogWarning("User with email '{Email}' not found.", model.Email);
            return new ConfirmEmailResultDTO(false, false);
        }

        if (user.EmailConfirmed)
        {
            _logger.LogInformation("The email '{Email}' is already confirmed.", model.Email);
            return new ConfirmEmailResultDTO(true, true);
        }
        _ = Task.Run(async () =>
        {
            var code = await  _userManager.GenerateEmailConfirmationTokenAsync(user);
            HttpContext? context = _httpContextAccessor.HttpContext;
            _emailHelper.GenerateVerificationEmail(code, context, user.Email, user.Id,
                user.FirstName);
            _logger.LogInformation(
                "Verification email with a new confirmation token has been send to '{Email}'",
                model.Email);
        });
        return new ConfirmEmailResultDTO(true, false);
    }
}
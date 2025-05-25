using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TicketAPI.Data.Models;
using TicketAPI.Services.DTO;
using TicketAPI.Services.Helper;

namespace TicketAPI.Services.Scoped;

/// <summary>
/// Manages Authentification.
/// </summary>
public class AuthService(UserManager<ApplicationUser> _userManager, IHttpContextAccessor _httpContextAccessor, ITokenGenerator _tokenGenerator, ILogger<AuthService> _logger, EmailHelper _emailHelper, IMfaService _mfaService)
{
    /// <summary>
    /// Logs the user in and creates an auth token.
    /// </summary>
    /// <param name="model">Email and password.</param>
    /// <returns>A result with the auth token on success.</returns>
    public async Task<LoginResultDTO?> LoginAsync(LoginModelDTO model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user == null)
        {
            _logger.LogWarning("User with Email '{Email}' not found.", model.Email);
            return null;
        }
        else if (!await _userManager.CheckPasswordAsync(user, model.Password))
        {
            _logger.LogWarning("Password incorrect.");
            return null;
        }

        if (!user.EmailConfirmed)
        {
            _logger.LogWarning("The email has not been confirmed yet.");
            return new LoginResultDTO() { IsEmailConfirmed = false };
        }
        
        if (user.TwoFactorEnabled)
        {
            return new LoginResultDTO() { IsMfaEnabled = true, IsEmailConfirmed = true };
        }

        var token = await _tokenGenerator.GenerateToken(user);
        _logger.LogInformation("Login for email '{Email}' successfull and token was generated.", model.Email);
        return new LoginResultDTO() { Token = token, IsEmailConfirmed = true };
    }
    
    public async Task<LoginResultDTO?> MfaLoginAsync(MfaVerifyDTO model)
    {
        var user = await _userManager.FindByEmailAsync(model.UserEmail);

        if (user == null)
        {
            _logger.LogWarning("User with Email '{Email}' not found.", model.UserEmail);
            return null;
        }

        if (!user.EmailConfirmed)
        {
            _logger.LogWarning("The email has not been confirmed yet.");
            return new LoginResultDTO() { IsEmailConfirmed = false };
        }

        var token = await _tokenGenerator.GenerateToken(user);
        _logger.LogInformation("Login for email '{Email}' successfull and token was generated.", model.UserEmail);
        return new LoginResultDTO() { Token = token, IsEmailConfirmed = true };
    }

    /// <summary>
    /// Checks the user registration and sends a verification on success.
    /// </summary>
    /// <param name="model">Full name, email and password.</param>
    /// <returns>Success of the register and whether the repeat password matches the passsword.</returns>
    public async Task<RegisterResultDTO> Register(RegisterModelDTO model)
    {
        if (model.Password != model.RepeatPassword)
        {
            _logger.LogWarning("Password '{Password}' and repeat password '{RepeatPassword}' do not match.", model.Password, model.RepeatPassword);
            return new RegisterResultDTO(false, false);
        }
            
        var user = new ApplicationUser{ FirstName = model.FirstName, LastName = model.LastName, UserName  = model.Email, Email = model.Email };
        var result = await _userManager.CreateAsync(user, model.Password);
            
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "User");
            _ = Task.Run(async () =>
            {
                string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                HttpContext? context = _httpContextAccessor.HttpContext;
                _emailHelper.GenerateVerificationEmail(code, context, user.Email, user.Id,
                    user.FirstName);
            });
            _logger.LogInformation("User '{FirstName} {LastName}' has been created and the verification mail has been send.", user.FirstName, user.LastName);
            return new RegisterResultDTO(true, true);
        }
        foreach (var error in result.Errors)
        {
            _logger.LogError("Error: {Code} - {Description}", error.Code, error.Description);
        }
        return new RegisterResultDTO(true, false);
    }

    /// <summary>
    /// Deletes the matching user.
    /// </summary>
    /// <param name="model">The eamil of the user to delete.</param>
    /// <returns>The Task that represents the asynchronous operation, containing the identity result.</returns>
    public async Task DeleteUser(ResendConfirmationEmailModelDTO model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        await _userManager.DeleteAsync(user);
    }

    public async Task<byte[]?> MfaSetup(ClaimsPrincipal userClaimPrincipal)
    {
        var user = await _userManager.GetUserAsync(userClaimPrincipal);
        if (user.TwoFactorEnabled)
            return null;

        var key = await _mfaService.GenerateNewAuthenticatorKeyAsync(user);
        var qr = await _mfaService.GenerateQrCodeAsync(user.Email, key);
        return qr;
    }

    public async Task<bool> MfaEnable(ClaimsPrincipal userClaimPrincipal, string code)
    {
        var user = await _userManager.GetUserAsync(userClaimPrincipal);
        if (await _mfaService.EnableAuthenticatorAsync(user, code))
            return true;
        return false;
    }

    public async Task<bool> MfaVerify(MfaVerifyDTO dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.UserEmail);
        if (user == null) 
            return false;
        
        if (!await _mfaService.VerifyTwoFactorTokenAsync(user, dto.Code))
            return false;
        
        return true;
    }
}
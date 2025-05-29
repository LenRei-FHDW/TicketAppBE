using System.Security.Claims;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using TicketAPI.Data.Models;
using TicketAPI.Services.DTO;
using TicketAPI.Services.Helper;

namespace TicketAPI.Services.Scoped;

/// <summary>
/// Manages Authentification.
/// </summary>
public class AuthService(UserManager<ApplicationUser> _userManager, IHttpContextAccessor _httpContextAccessor, ITokenGenerator _tokenGenerator, ILogger<AuthService> _logger, EmailHelper _emailHelper)
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

        var token = await _tokenGenerator.GenerateToken(user);
        _logger.LogInformation("Login for email '{Email}' successfull and token was generated.", model.Email);
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
        if (user != null) await _userManager.DeleteAsync(user);
    }
    
    public async Task<ApplicationUser> CreateOrLoginGoogleUserAsync(GoogleJsonWebSignature.Payload googlePayload)
    {
        var existingUser = await _userManager.FindByLoginAsync("Google", googlePayload.Subject);
        
        if (existingUser != null)
        {
            return existingUser;
        }
        var userByEmail = await _userManager.FindByEmailAsync(googlePayload.Email);
        
        if (userByEmail != null)
        {
            await LinkGoogleAccountAsync(userByEmail, googlePayload);
            return userByEmail;
        }
        var newUser = new ApplicationUser
        {
            UserName = googlePayload.Email,
            Email = googlePayload.Email,
            EmailConfirmed = true,
            FirstName = googlePayload.GivenName,
            LastName = googlePayload.FamilyName,
        };
        var createResult = await _userManager.CreateAsync(newUser);
        
        if (!createResult.Succeeded)
        {
            var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to create user: {errors}");
        }
        var loginInfo = new UserLoginInfo("Google", googlePayload.Subject, "Google");
        var addLoginResult = await _userManager.AddLoginAsync(newUser, loginInfo);
        
        if (!addLoginResult.Succeeded)
        {
            var errors = string.Join(", ", addLoginResult.Errors.Select(e => e.Description));
            _logger.LogError("Failed to add Google login to user {UserId}: {Errors}", newUser.Id, errors);
        }

        return newUser;
    }

    private async Task LinkGoogleAccountAsync(ApplicationUser user, GoogleJsonWebSignature.Payload googlePayload)
    {
        var existingLogins = await _userManager.GetLoginsAsync(user);
        var hasGoogleLogin =
            existingLogins.Any(x => x.LoginProvider == "Google" && x.ProviderKey == googlePayload.Subject);

        if (!hasGoogleLogin)
        {
            var loginInfo = new UserLoginInfo("Google", googlePayload.Subject, "Google");
            var result = await _userManager.AddLoginAsync(user, loginInfo);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to link Google account: {errors}");
            }
        }
    }
}
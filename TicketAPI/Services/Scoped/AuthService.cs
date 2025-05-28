using System.Security.Claims;
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

        public async Task<GoogleResultDTO> RegisterGoogle(AuthenticateResult googleAuthResult)
    {
        var info = new UserLoginInfo(
            GoogleDefaults.AuthenticationScheme,
            googleAuthResult.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new InvalidOperationException(),
            "Google"
        );
        var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
        bool isNewUser = false;

        if (user == null)
        {
            var email = googleAuthResult.Principal.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
            user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = MapClaimsToAppUser(googleAuthResult.Principal);

                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    _logger.LogError("Failed to create user: {Errors}", string.Join(", ", createResult.Errors.Select(e => e.Description)));
                    return new GoogleResultDTO() { Success = false, Message = "Failed to create user account" };
                }
                isNewUser = true;
            }
            var addLoginResult = await _userManager.AddLoginAsync(user, info);
            if (!addLoginResult.Succeeded)
            {
                _logger.LogError("Failed to add Google login: {Errors}", string.Join(", ", addLoginResult.Errors.Select(e => e.Description)));
                return new GoogleResultDTO { Success = false, Message = "Failed to link Google account" };
            }
        }
        var token = await _tokenGenerator.GenerateToken(user);

        return new GoogleResultDTO
        {
            Success = true,
            Token = token,
            Message = isNewUser ? "Registration successful" : "Login successful"
        };
    }

    private static ApplicationUser MapClaimsToAppUser(ClaimsPrincipal user)
    {
        return new ApplicationUser
        {
            UserName = user.FindFirst(ClaimTypes.Email)?.Value,
            Email = user.FindFirst(ClaimTypes.Email)?.Value,
            EmailConfirmed = true,
            FirstName = user.FindFirst(ClaimTypes.GivenName)?.Value ?? string.Empty,
            LastName = user.FindFirst(ClaimTypes.Surname)?.Value ?? string.Empty,
        };
    }
}
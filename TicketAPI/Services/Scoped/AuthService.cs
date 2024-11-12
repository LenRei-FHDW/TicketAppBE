using Microsoft.AspNetCore.Identity;
using TicketAPI.Data.Models;
using TicketAPI.Services.DTO;
using TicketAPI.Services.Transient;

namespace TicketAPI.Services.Scoped;

public class AuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly EmailHelper _emailHelper;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly ILogger<AuthService> _logger;


    public AuthService(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor, ITokenGenerator tokenGenerator, ILogger<AuthService> logger, EmailHelper emailHelper)
    {
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
        _tokenGenerator = tokenGenerator;
        _logger = logger;
        _emailHelper = emailHelper;
    }


    public async Task<LoginResultDTO?> LoginAsync(LoginModelDTO model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
        {
            return null;
        }

        if (!user.EmailConfirmed)
        {
            return new LoginResultDTO() { IsEmailConfirmed = false };
        }

        var token = _tokenGenerator.GenerateToken(user);
        return new LoginResultDTO() { Token = token, IsEmailConfirmed = true };
    }

    public async Task<RegisterResultDTO> Register(RegisterModelDTO model)
    {
        if (model.Password != model.RepeatPassword)
        {
            return new RegisterResultDTO(false, false);
        }
            
        var user = new ApplicationUser{ FirstName = model.FirstName, LastName = model.LastName, UserName  = model.Email, Email = model.Email };
        var result = await _userManager.CreateAsync(user, model.Password);
            
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "User");
            string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            HttpContext? context = _httpContextAccessor.HttpContext;
            _emailHelper.GenerateVerificationEmail(code, context, user.Email, user.Id);
            return new RegisterResultDTO(true, true);
        }
        foreach (var error in result.Errors)
        {
            Console.WriteLine($"Error: {error.Code} - {error.Description}");
        }
        return new RegisterResultDTO(true, false);
    }

    public async Task DeleteUser(ResendConfirmationEmailModelDTO model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        await _userManager.DeleteAsync(user);
    }
}
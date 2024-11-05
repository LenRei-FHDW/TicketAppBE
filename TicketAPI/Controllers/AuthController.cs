using System.CodeDom.Compiler;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TicketAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TicketAPI.ViewModels;

namespace TicketAPI.Controllers;

public class AuthController : Controller
{
    
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;

    public AuthController(UserManager<ApplicationUser> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }
    
    // POST
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDTO model)
    {
        
        var user = await _userManager.FindByEmailAsync(model.email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, model.password))
        {
            return Unauthorized();
        }
        
        var token = GenerateJwtToken(user);
        return Ok(new { 
            firstName = user.FirstName,
            lastName = user.LastName,
            token = token 
        });
        
    }
    
    // POST
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegistrationDTO model)
    {
        if (model.Password != model.RepeatPassword)
        {
            return BadRequest(model);
        }
        
        var user = new ApplicationUser(model.FirstName, model.LastName, model.Username, model.Email);
        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }
        
        await _userManager.AddToRoleAsync(user, "User");

        return Ok();
    }
    
    private string GenerateJwtToken(ApplicationUser user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };
        
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var token = new JwtSecurityToken(
            jwtSettings["Issuer"],
            jwtSettings["Audience"],
            claims,
            expires: DateTime.Now.AddMinutes(double.Parse(jwtSettings["ExpireMinutes"])),
            signingCredentials: creds);
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
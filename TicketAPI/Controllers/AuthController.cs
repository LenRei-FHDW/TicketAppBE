using System.CodeDom.Compiler;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TicketAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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
    public async Task<IActionResult> Login(string email, string password)
    {
        
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, password))
        {
            return Unauthorized();
        }
        
        var token = GenerateJwtToken(user);
        return Ok(new { token = token });
        
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
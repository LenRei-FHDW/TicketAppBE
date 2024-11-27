using Microsoft.AspNetCore.Identity;
using TicketAPI.Data.Models;

namespace TicketAPI.Data;

/// <summary>
/// This Class can SeedData to the Database
/// </summary>
public class DbSeeder
{
    /// <summary>
    /// Seed Rollen into the Databse
    /// </summary>
    /// <param name="roleManager">RoleManger to create Roles</param>
    public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        string [] roleNames = ["Admin", "User"];

        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }
    
    /// <summary>
    /// Seed Admin User into the Databse
    /// </summary>
    /// <param name="userManager">UserManger to create User</param>
    /// <param name="configuration">Configuration from appsetings</param>
    public static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager, IConfiguration configuration)
    {
        var adminEmail = configuration["AdminUser:Email"];
        var adminPassword = configuration["AdminUser:Password"];
    
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
 
            var newAdminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };
            var createAdminResult = await userManager.CreateAsync(newAdminUser, adminPassword);
        
            if (createAdminResult.Succeeded)
            {
                await userManager.AddToRoleAsync(newAdminUser, "Admin");
            }
            else
            {
                throw new Exception(createAdminResult.Errors.First().Description);
            }
        }
    }
}
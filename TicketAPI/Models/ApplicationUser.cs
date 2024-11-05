using Microsoft.AspNetCore.Identity;

namespace TicketAPI.Models;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = String.Empty;
    public string LastName { get; set; } = String.Empty;

    public ApplicationUser()
    {
        
    }
    
    public ApplicationUser(string firstName, string lastName, string username, string email, bool emailConfirmed = false)
    {
        FirstName = firstName;
        LastName = lastName;
        this.UserName = username;
        this.Email = email;
        this.EmailConfirmed = emailConfirmed;
    }
}
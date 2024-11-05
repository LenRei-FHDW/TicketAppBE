using Microsoft.AspNetCore.Identity;

namespace TicketAPI.Models;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = String.Empty;
    public string LastName { get; set; } = String.Empty;

    public ApplicationUser(string firstName, string lastName, string email)
    {
        FirstName = firstName;
        LastName = lastName;
        this.Email = email;
    }
}
using Microsoft.EntityFrameworkCore;
using TicketAPI.Data.Models;

namespace TicketAPI.Data.Repositories;

public interface IUserRepository
{
    Task<ApplicationUser?> GetUserWithAddressAsync(string userId);
    Task UpdateUserAsync(ApplicationUser user);
}

/// <summary>
/// Manages User Database interaction.
/// </summary>
public class UserRepository(TicketApiDbContext _context) : IUserRepository
{
    /// <summary>
    /// Call the Database with userId
    /// </summary>
    /// <param name="userId">UserId from User</param>
    /// <returns>ApllicationUser Model with Address Model</returns>
    public async Task<ApplicationUser?> GetUserWithAddressAsync(string userId)
    {
        return await _context.Users
            .Include(u => u.Addresse)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }

    /// <summary>
    /// Update ApplicationUser in Database
    /// </summary>
    /// <param name="user">Application Model</param>
    public async Task UpdateUserAsync(ApplicationUser user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }
}
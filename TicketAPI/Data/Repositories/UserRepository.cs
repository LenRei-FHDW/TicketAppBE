using Microsoft.EntityFrameworkCore;
using TicketAPI.Data.Models;

namespace TicketAPI.Data.Repositories;

public interface IUserRepository : IRepository<ApplicationUser, string>
{
    Task<ApplicationUser?> GetUserWithAddressAsync(string userId);
}

/// <summary>
/// Manages User Database interaction.
/// </summary>
public class UserRepository(TicketApiDbContext _context) : Repository<ApplicationUser, string>(_context), IUserRepository
{
    /// <summary>
    /// Call the Database with userId
    /// </summary>
    /// <param name="userId">UserId from User</param>
    /// <returns>ApplicationUser Model with Address Model</returns>
    public async Task<ApplicationUser?> GetUserWithAddressAsync(string userId)
    {
        return await _context.Users
            .Include(u => u.Addresse)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }
}
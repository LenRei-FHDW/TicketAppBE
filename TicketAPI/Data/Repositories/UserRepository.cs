using Microsoft.EntityFrameworkCore;
using TicketAPI.Data.Models;

namespace TicketAPI.Data.Repositories;

public interface IUserRepository
{
    Task<ApplicationUser?> GetUserWithAddressAsync(string userId);
    Task UpdateUserAsync(ApplicationUser user);
}

public class UserRepository(TicketApiDbContext _context) : IUserRepository
{
    
    public async Task<ApplicationUser?> GetUserWithAddressAsync(string userId)
    {
        return await _context.Users
            .Include(u => u.Addresse)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task UpdateUserAsync(ApplicationUser user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }
}
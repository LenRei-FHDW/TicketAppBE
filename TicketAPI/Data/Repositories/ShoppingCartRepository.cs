using Microsoft.EntityFrameworkCore;
using TicketAPI.Data.Models;

namespace TicketAPI.Data.Repositories;

public class ShoppingCartRepository : Repository<ShoppingCartItem, Guid>
{
    public ShoppingCartRepository(TicketApiDbContext _dbContext) : base(_dbContext) { }
    
    public async Task<ShoppingCartItem> GetShoppingCartItemWhereUserIdAndProductId(string userId,
        Guid productId)
    {
        return await _context.ShoppingCartItems
            .FirstOrDefaultAsync(ci => ci.ApplicationUserId == userId && ci.ProductId == productId) ?? throw new KeyNotFoundException();
    }

    public async Task<IEnumerable<ShoppingCartItem>> GetShoppingCartItemWhereUserIdJoinProduct(
        string userId)
    {
        return await _dbSet
            .Include(ci => ci.Product)
            .Where(ci => ci.ApplicationUserId == userId)
            .ToListAsync();
    }
}
using Microsoft.EntityFrameworkCore;
using TicketAPI.Data.Models;

namespace TicketAPI.Data.Repositories;

public class ShoppingCartRepository(TicketApiDbContext context) : Repository<ShoppingCartItem, Guid>(context)
{
    
    public async Task<ShoppingCartItem> GetShoppingCartItemWhereUserIdAndProductId(string userId,
        Guid productId)
    {
        return await _dbSet
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
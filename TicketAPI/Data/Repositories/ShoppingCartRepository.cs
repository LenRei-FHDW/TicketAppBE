using Microsoft.EntityFrameworkCore;
using TicketAPI.Data.Models;

namespace TicketAPI.Data.Repositories;

public class ShoppingCartRepository(TicketApiDbContext context) : Repository<ShoppingCartItem, Guid>(context)
{
    /// <summary>
    /// Get ShoppingCartItem from Database
    /// </summary>
    /// <param name="userId">Id of User</param>
    /// <param name="productId">Id of Product</param>
    /// <returns>ShoppingCartItem from Database</returns>
    public async Task<ShoppingCartItem> GetShoppingCartItemWhereUserIdAndProductId(string userId,
        Guid productId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(ci => ci.ApplicationUserId == userId && ci.ProductId == productId) ?? throw new KeyNotFoundException();
    }

    /// <summary>
    /// Get IEnumerable ShoppingCartItem from Database
    /// </summary>
    /// <param name="cartItemDto">Item to edit</param>
    /// <returns>IEnumerable ShoppingCartItem from Database</returns>
    public async Task<IEnumerable<ShoppingCartItem>> GetShoppingCartItemWhereUserIdJoinProduct(
        string userId)
    {
        return await _dbSet
            .Include(ci => ci.Product)
            .Where(ci => ci.ApplicationUserId == userId)
            .ToListAsync();
    }
}
using Microsoft.EntityFrameworkCore;
using TicketAPI.Data.Models;

namespace TicketAPI.Data.Repositories;

public interface IOrderRepository : IRepository<Order, Guid>
{
    Task<IEnumerable<Order>> GetByUserIdJoinOrderItemsJoinProductsAsync(string userId);
    Task<Order> GetByIdAsyncLoadEager(Guid orderId);
}

public class OrderRepository(TicketApiDbContext context) : Repository<Order, Guid>(context), IOrderRepository
{
    public async Task<IEnumerable<Order>> GetByUserIdJoinOrderItemsJoinProductsAsync(string userId)
    {
        return await _context.Orders
            .Where(o => o.ApplicationUserId == userId)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .ToListAsync();
    }
    
    public async Task<Order> GetByIdAsyncLoadEager(Guid orderId)
    {
            return await _dbSet
                .Include(e => e.ApplicationUser)
                .Include(e => e.OrderItems)
                .ThenInclude(e => e.Product)
                .FirstOrDefaultAsync(e => e.OrderId == orderId) ?? throw new KeyNotFoundException();
    }
}
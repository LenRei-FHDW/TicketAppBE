using Microsoft.EntityFrameworkCore;
using TicketAPI.Data.Models;

namespace TicketAPI.Data.Repositories;

public class OrderRepository(TicketApiDbContext context) : Repository<Order, Guid>(context)
{
    public async Task<Order> GetByIdAsynchLoadEager(Guid orderId)
    {
            return await _dbSet
                .Include(e => e.ApplicationUser)
                .Include(e => e.OrderItems)
                .ThenInclude(e => e.Product)
                .FirstOrDefaultAsync(e => e.OrderId == orderId) ?? throw new KeyNotFoundException();
    }

    public async Task<Order> GetByIdJoinOrderItems(Guid orderId)
    {
        return await _dbSet
            .Include(e => e.OrderItems)
            .FirstOrDefaultAsync(e => e.OrderId == orderId) ?? throw new KeyNotFoundException();
    }
}
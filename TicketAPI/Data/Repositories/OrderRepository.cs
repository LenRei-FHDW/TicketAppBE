using Microsoft.EntityFrameworkCore;
using TicketAPI.Data.Models;

namespace TicketAPI.Data.Repositories;

public interface IOrderRepository
{
    Task<Order> GetByIdAsynchLoadEager(Guid orderId);
}

public class OrderRepository(TicketApiDbContext context) : Repository<Order, Guid>(context), IOrderRepository
{
    public async Task<Order> GetByIdAsyncLoadEager(Guid orderId)
    {
            return await _dbSet
                .Include(e => e.ApplicationUser)
                .Include(e => e.OrderItems)
                .ThenInclude(e => e.Product)
                .FirstOrDefaultAsync(e => e.OrderId == orderId) ?? throw new KeyNotFoundException();
    }
}
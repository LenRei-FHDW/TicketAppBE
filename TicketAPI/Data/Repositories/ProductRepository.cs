using Microsoft.EntityFrameworkCore;
using TicketAPI.Data.Models;

namespace TicketAPI.Data.Repositories;

public class ProductRepository(TicketApiDbContext context) : Repository<Product, Guid>(context)
{
    public async Task<IEnumerable<Product>> GetAllWhereNotDeletedAsync()
    {
        return await _dbSet.Where(p => p.IsDeleted == false).ToListAsync();
    }
}

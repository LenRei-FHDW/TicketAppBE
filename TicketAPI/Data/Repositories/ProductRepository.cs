using Microsoft.EntityFrameworkCore;
using TicketAPI.Data.Models;

namespace TicketAPI.Data.Repositories;

public interface IProductRepository : IRepository<Product, Guid>
{
    public Task<List<Product>> GetProductsWhereCategoryIdAsync(Guid categoryId);
}

/// <summary>
/// Manages product Database interaction.
/// </summary>
public class ProductRepository(TicketApiDbContext context) : Repository<Product, Guid>(context), IProductRepository
{
    /// <summary>
    /// Call the Database for all Product where nit deletet
    /// </summary>
    /// <returns>Returns a List of Products</returns>
    public new async Task<List<Product>> GetAllAsync()
    {
        return await _dbSet
            .Where(p => !p.IsDeleted)
            .ToListAsync() ?? throw new KeyNotFoundException();
    }
    
    public async Task<List<Product>> GetProductsWhereCategoryIdAsync(Guid categoryId)
    {
        return await _dbSet
            .Where(p => p.CategoryId == categoryId && !p.IsDeleted)
            .ToListAsync();
    }
}
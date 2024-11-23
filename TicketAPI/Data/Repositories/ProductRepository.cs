using Microsoft.EntityFrameworkCore;
using TicketAPI.Data.Models;

namespace TicketAPI.Data.Repositories;

public interface IProductRepository
{
    public Task<List<Product>> GetProductsWhereCategoryIdAsync(Guid categoryId);
}

/// <summary>
/// Manages product Database interaction.
/// </summary>
public class ProductRepository(TicketApiDbContext _context) : IProductRepository
{
    /// <summary>
    /// Call the Database for all Product where nit deletet
    /// </summary>
    /// <returns>Returns a List of Products</returns>
    public async Task<List<Product>> GetProductsAsync()
    {
        return await _context.Products
            .Where(p => !p.IsDeleted)
            .ToListAsync() ?? throw new KeyNotFoundException();
    }
    
    public async Task<List<Product>> GetProductsWhereCategoryIdAsync(Guid categoryId)
    {
        return await _dbSet
            .Where(p => p.CategoryId == categoryId)
            .ToListAsync();
    }
}
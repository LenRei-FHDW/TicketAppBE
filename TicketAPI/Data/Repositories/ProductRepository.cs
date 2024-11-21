using Microsoft.EntityFrameworkCore;
using TicketAPI.Data.Models;

namespace TicketAPI.Data.Repositories;

public interface IProductRepository
{
    Task<List<Product>> GetProductsAsync();
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
}
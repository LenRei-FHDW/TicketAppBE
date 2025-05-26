using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TicketAPI.Data.Models;

namespace TicketAPI.Data;

public class TicketApiDbContext(DbContextOptions<TicketApiDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Category> Categories { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        Guid categoryIdSaison = Guid.NewGuid();
        Guid categoryIdFamilie = Guid.NewGuid(); 
        
        modelBuilder.Entity<ShoppingCartItem>()
            .HasKey(ci => new { ci.ApplicationUserId, ci.ProductId });

        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .OnDelete(DeleteBehavior.SetNull);
        
        modelBuilder.Entity<Category>().HasData(
            new Category
            {
                CategoryId = Guid.NewGuid(),
                Name = "Kinder"
            },
            new Category
            {
                CategoryId = categoryIdSaison,
                Name = "Saisonticket"
            },
            new Category
            {
                CategoryId = categoryIdFamilie,
                Name = "Familie"
            }
        );
        // Seed data for the Products table
        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                ProductId = Guid.NewGuid(),
                Name = "Sample Product 1",
                Description = "This is the first sample product.",
                Price = 19.99m,
                ImageName = "sample1.jpg",
                IsDeleted = false,
                CreaterId = "user-123",
                CategoryId = categoryIdFamilie,
            },
            new Product
            {
                ProductId = Guid.NewGuid(),
                Name = "Sample Product 2",
                Description = "This is the second sample product.",
                Price = 29.99m,
                ImageName = "sample2.jpg",
                IsDeleted = false,
                CreaterId = "user-456",
                CategoryId = categoryIdSaison
            }
        );
        base.OnModelCreating(modelBuilder);
    }
}
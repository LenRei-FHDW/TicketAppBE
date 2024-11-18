using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TicketAPI.Data.Models;

namespace TicketAPI.Data;

public class TicketApiDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    
    public TicketApiDbContext(DbContextOptions<TicketApiDbContext> options) : base(options) {}
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
        modelBuilder.Entity<ShoppingCartItem>()
            .HasKey(ci => new { ci.ApplicationUserId, ci.ProductId });

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
                Rating = 4.5m
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
                Rating = 4.0m
            }
        );
        base.OnModelCreating(modelBuilder);
    }
}
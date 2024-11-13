using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TicketAPI.Data.Models;

namespace TicketAPI.Data;

public class TicketApiDbContext : IdentityDbContext<ApplicationUser>
{
    DbSet<Ticket> tickets;
    DbSet<Order> orders;
    DbSet<OrderItem> orderItems;
    public TicketApiDbContext(DbContextOptions<TicketApiDbContext> options) : base(options) {}
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);  
        
    }
}
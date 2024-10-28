using Microsoft.EntityFrameworkCore;

namespace TicketAPI.Data;

public class TicketApiDbContext : DbContext
{
    public TicketApiDbContext(DbContextOptions<TicketApiDbContext> options) : base(options) {}
}
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TicketAPI.Data.Models;

namespace TicketAPI.Data;

public class TicketApiDbContext : IdentityDbContext<ApplicationUser>
{
    public TicketApiDbContext(DbContextOptions<TicketApiDbContext> options) : base(options) {}
}
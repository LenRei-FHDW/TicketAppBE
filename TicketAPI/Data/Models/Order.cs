using System.ComponentModel.DataAnnotations.Schema;

namespace TicketAPI.Data.Models;

public class Order
{
    public int OrderId { get; set; }
    public DateTime CreatedAt { get; set; }
    public ApplicationUser User { get; set; } = null!;
    public List<OrderItem> Items { get; } = [];

    [NotMapped]
    public decimal TotalPrice => Items.Sum(i => i.Quantity * i.Ticket.Price);
}
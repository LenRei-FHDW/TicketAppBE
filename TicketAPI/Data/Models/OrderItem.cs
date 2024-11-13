namespace TicketAPI.Data.Models;

public class OrderItem
{
    public int OrderItemId { get; set; }
    public Order Order { get; set; } = null!;
    public Ticket Ticket { get; set; } = null!;
    public int Quantity { get; set; }
}
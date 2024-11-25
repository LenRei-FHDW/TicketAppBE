namespace TicketAPI.Services.DTO;

public class OrderItemDTO
{
    public Guid OrderItemId { get; set; }
    public Guid ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal SinglePrice { get; set; }
    public decimal TotalPrice { get; set; }
}
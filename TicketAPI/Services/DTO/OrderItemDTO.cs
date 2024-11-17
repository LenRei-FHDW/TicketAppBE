namespace TicketAPI.Services.DTO;

public class OrderItemDTO
{
    public Guid ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal SinglePrice { get; set; }
    public decimal TotalPrice { get; set; }
}
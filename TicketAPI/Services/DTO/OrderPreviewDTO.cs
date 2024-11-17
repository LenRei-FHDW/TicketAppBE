namespace TicketAPI.Services.DTO;

public class OrderPreviewDTO
{
    public Guid OrderId { get; set; }
    public DateTime CreatedAt { get; set; }
    public decimal TotalPrice { get; set; }
}
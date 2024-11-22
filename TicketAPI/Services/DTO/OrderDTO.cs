namespace TicketAPI.Services.DTO;

public class OrderDTO
{
    public Guid OrderId { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public IEnumerable<OrderItemDTO> OrderItems { get; } = new List<OrderItemDTO>();
    
    public int ProductCount { get; set; }
    public decimal TotalPrice { get; set; }
}
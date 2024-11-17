namespace TicketAPI.Services.DTO;

public class OrderItemPostDTO
{
    public Guid ProductId { get; init; }
    public int Quantity { get; init; }
}
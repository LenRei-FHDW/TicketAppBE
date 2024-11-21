namespace TicketAPI.Services.DTO;

public class ProductPreviewDTO
{
    public Guid ProductId { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string ImageName { get; set; }
}
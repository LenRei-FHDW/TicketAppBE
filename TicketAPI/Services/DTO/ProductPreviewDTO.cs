namespace TicketAPI.Services.DTO;

public class ProductPreviewDTO
{
    public Guid ProductId { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string ImageName { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? CategoryID { get; set; }
    public string CategoryName { get; set; }
}
namespace TicketAPI.Services.DTO;

public class ProductEditDTO
{
    public Guid ProductId { get; set; }
    
    public string Name {get; set;}
    
    public string Description {get; set;}
    
    public decimal Price {get; set;}
    
    public string? ImageName { get; set; }
    
    public IFormFile? ImageFile {get; set;}
    
    public Guid? CategoryId {get; set;}
}
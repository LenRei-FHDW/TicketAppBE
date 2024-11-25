namespace TicketAPI.Services.DTO;

public class ProductCreateDTO
{
    public string Name {get; set;}
    
    public string Description {get; set;}
    
    public decimal Price {get; set;}
    
    public IFormFile? ImageFile {get; set;}
    
    public Guid? CategoryId {get; set;}
}
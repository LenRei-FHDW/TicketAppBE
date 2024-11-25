namespace TicketAPI.Services.DTO;

public class ProductDTO{
    public Guid ProductId { get; set; }

    public string Name {get; set;} = String.Empty;
    
    public string Description {get; set;} =  String.Empty;
    
    public decimal Price {get; set;}
    
    public string ImageName {get; set;} 
    
    public DateTime CreatedAt { get; set; }
    
    public Guid? CategoryId {get; set;}
}
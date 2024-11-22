using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketAPI.Data.Models.DTO;

public class ShoppingCartItemDTO
{
    public Guid ProductId { get; set; }
    
    public string ProductName { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Range(0, double.MaxValue)]
    public decimal Price {get; set;}
    
    public string Description {get; set;}
    
    public string? ImageName { get; set; }

    public int Quantity { get; set; }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketAPI.Data.Models;

public class Product
{
    [Key]
    public int ProductId { get; set; }
    
    [Required]
    public string Name {get; set;} = String.Empty;
    
    
    public string Description {get; set;} =  String.Empty;
   
    [Column(TypeName = "decimal(18,2)")]
    [Range(0, double.MaxValue)]
    public decimal Price {get; set;}
    
    public string? ImageName { get; set; }
    
    public bool IsDeleted { get; set; } = false;
    
    public string CreaterId { get; set; }
    
    public decimal Rating {get; set;}
    
    
    
}
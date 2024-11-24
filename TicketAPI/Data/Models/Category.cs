using System.ComponentModel.DataAnnotations;

namespace TicketAPI.Data.Models;

public class Category
{
    [Key]
    public Guid CategoryId { get; set; }
    [Required]
    public string Name { get; set; }
    
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
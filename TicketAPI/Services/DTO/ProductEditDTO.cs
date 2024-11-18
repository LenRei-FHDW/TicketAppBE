using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketAPI.Services.DTO;

public class ProductEditDTO
{
    [Required]
    public Guid ProductId { get; set; }
    [Required]
    public string Name { get; set; }
    [Column(TypeName = "decimal(18,2)")]
    [Required]
    public decimal Price { get; set; }
    [Required]
    public string Description { get; set; }
    public IFormFile? ImageFile { get; set; }
}
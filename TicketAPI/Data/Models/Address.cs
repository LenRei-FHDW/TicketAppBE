using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace TicketAPI.Data.Models;

public class Address
{
    [Key]
    public Guid AddressId { get; set; } = Guid.NewGuid();
    
    public string ApplicationUserId { get; set; }
    [ForeignKey("ApplicationUserId")]
    
    public ApplicationUser ApplicationUser { get; set; }

    [Required]
    public string? Street1 { get; set; } = string.Empty;
    
    public string? Street2 { get; set; } = string.Empty;

    [Required]
    public string? City { get; set; } = string.Empty;
    
    public string? State { get; set; } = string.Empty;

    [Required]
    public string? Zip { get; set; } = string.Empty;
}
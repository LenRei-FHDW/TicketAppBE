using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TicketAPI.Data.Models;

public class Address
{
    [Key]
    public Guid AddressId { get; set; } = Guid.NewGuid();
    
    public string ApplicationUserId { get; set; }
    [ForeignKey("ApplicationUserId")]
    [JsonIgnore] 
    public ApplicationUser ApplicationUser { get; set; }

    [Required]
    public string Street { get; set; } = string.Empty;

    [Required]
    public string City { get; set; } = string.Empty;

    [Required]
    public string State { get; set; } = string.Empty;

    [Required]
    public string Zip { get; set; } = string.Empty;

    [Required]
    public string Country { get; set; } = string.Empty;
}
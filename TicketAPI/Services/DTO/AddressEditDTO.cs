using System.ComponentModel.DataAnnotations;

namespace TicketAPI.Services.DTO;

public class AddressEditDTO
{
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
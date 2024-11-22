using System.ComponentModel.DataAnnotations;

namespace TicketAPI.Data.Models.DTO;

public class ShoppingCartItemCreateDTO
{
    [Required]
    public Guid ProductId { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Die Menge muss mindestens 1 betragen.")]
    public int Quantity { get; set; } = 1;
}
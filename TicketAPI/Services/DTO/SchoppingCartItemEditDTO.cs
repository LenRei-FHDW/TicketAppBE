using System.ComponentModel.DataAnnotations;

namespace TicketAPI.Services.DTO;

public class SchoppingCartItemEditDTO
{
    [Range(1, int.MaxValue, ErrorMessage = "Die Menge muss mindestens 1 betragen.")]
    public int Quantity { get; set; }
}
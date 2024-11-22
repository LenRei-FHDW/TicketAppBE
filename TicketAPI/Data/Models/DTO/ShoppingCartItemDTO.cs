namespace TicketAPI.Data.Models.DTO;

public class ShoppingCartItemDTO
{
    public ProductPreview Product { get; set; }
    int Quantity { get; set; }
}
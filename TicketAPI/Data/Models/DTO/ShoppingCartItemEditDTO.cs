using System.ComponentModel.DataAnnotations;

public class ShoppingCartItemEditDTO
{
    [Range(0, int.MaxValue, ErrorMessage = "Die Menge muss mindestens 0 betragen.")]
    public int Quantity { get; set; }
}
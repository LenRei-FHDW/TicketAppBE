using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TicketAPI.Data.Models;

public class ShoppingCartItem
{
    public Guid ApplicationUserId { get; set; }
    [ForeignKey("ApplicationUserId")]
    [JsonIgnore] 
    public ApplicationUser ApplicationUser { get; set; }

    public Guid ProductId { get; set; }
    [ForeignKey("ProductId")]
    [JsonIgnore] 
    public Product Product { get; set; }

    [Required, Range(1, int.MaxValue)]
    public int Quantity { get; set; } = 1;
}
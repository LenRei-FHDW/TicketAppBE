using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TicketAPI.Data.Models;

public class OrderItem
{
    [Key]
    public Guid OrderItemId { get; set; } = Guid.NewGuid();
    
    public Guid OrderID { get; set; }
    [ForeignKey("OrderID")]
    public Order Order { get; set; }
    
    public int ProductID { get; set; }
    [ForeignKey("ProductID")]
    public Product Product { get; set; }
    
    [Required, Range(1, int.MaxValue)]
    public int Quantity { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    [Range(0, double.MaxValue)]
    public decimal SinglePrice { get; set; }
    
    [NotMapped]
    public decimal TotalPrice => SinglePrice * Quantity;
}
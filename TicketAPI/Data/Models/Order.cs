using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketAPI.Data.Models;

public class Order
{
    [Key]
    public Guid OrderId { get; set; } = Guid.NewGuid();
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public Guid ApplicationUserId { get; set; }
    [ForeignKey("ApplicationUserId")]
    public ApplicationUser ApplicationUser { get; set; }
    public ICollection<OrderItem> OrderItems { get; } = new List<OrderItem>();
    
    [NotMapped]
    public decimal TotalPrice => OrderItems.Sum(i => i.TotalPrice);
}
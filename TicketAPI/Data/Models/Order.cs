using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TicketAPI.Data.Models;

public class Order
{
    [Key]
    public Guid OrderId { get; set; } = Guid.NewGuid();
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public Guid ApplicationUserId { get; set; }
    [ForeignKey("ApplicationUserId")]
    [JsonIgnore] 
    public ApplicationUser ApplicationUser { get; set; }
    
    [JsonIgnore] 
    public ICollection<OrderItem> OrderItems { get; } = new List<OrderItem>();
    
    [NotMapped]
    public decimal TotalPrice => OrderItems.Sum(i => i.TotalPrice);
}
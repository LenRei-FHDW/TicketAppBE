using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketAPI.Data.Models;

public class Order
{
    [Key]
    public Guid OrderId { get; set; } = Guid.NewGuid();
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public string ApplicationUserId { get; set; }
    [ForeignKey("ApplicationUserId")]
    public ApplicationUser ApplicationUser { get; set; }
    public ICollection<OrderItem> OrderItems { get; } = new List<OrderItem>();

    public string StripeId { get; set; } = string.Empty;

    public PlaymentStatus PlaymentStatus { get; set; } = PlaymentStatus.Open;
    
    [NotMapped]
    public decimal TotalPrice => OrderItems.Sum(i => i.TotalPrice);
}

public enum PlaymentStatus
{
    Open,
    Canceled,
    Success
} 
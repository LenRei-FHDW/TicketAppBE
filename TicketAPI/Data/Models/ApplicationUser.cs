using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace TicketAPI.Data.Models;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = String.Empty;
    public string LastName { get; set; } = String.Empty;
   
    [JsonIgnore] 
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    [JsonIgnore] 
    public ICollection<Address> Addresses { get; set; } = new List<Address>();
    [JsonIgnore] 
    public ICollection<ShoppingCartItem> ShoppingCartItems { get; set; } = new List<ShoppingCartItem>();
}
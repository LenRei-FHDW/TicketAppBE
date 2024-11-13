namespace TicketAPI.Data.Models;

public class Ticket
{
    public int TicketId { get; set; }
    public string Name {get; set;} = String.Empty;
    public string Description {get; set;} =  String.Empty;
    public decimal Price {get; set;}
    public decimal Rating {get; set;}
}
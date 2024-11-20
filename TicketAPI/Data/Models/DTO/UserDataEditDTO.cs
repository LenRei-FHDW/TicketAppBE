namespace TicketAPI.Data.Models.DTO;

public class UserDataEditDTO
{
    public string ApplicationUserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Street { get; set; }
    public string City { get; set; }
    public string Zip { get; set; }
}
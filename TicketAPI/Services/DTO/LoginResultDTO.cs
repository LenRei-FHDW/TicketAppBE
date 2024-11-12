namespace TicketAPI.Services.DTO;

public class LoginResultDTO
{
    public string? Token { get; set; }
    public bool IsEmailConfirmed { get; set; }
}
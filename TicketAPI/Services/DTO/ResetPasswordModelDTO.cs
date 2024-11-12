namespace TicketAPI.Services.DTO;

public class ResetPasswordModelDTO
{
    public string Email { get; set; }
    public string Token { get; set; }
    public string NewPassword { get; set; }
}
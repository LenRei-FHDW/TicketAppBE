namespace TicketAPI.ViewModels;

public class ResetPasswordModelDTO
{
    public string Email { get; set; }
    public string Token { get; set; }
    public string NewPassword { get; set; }
}
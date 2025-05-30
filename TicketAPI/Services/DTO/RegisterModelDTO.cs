namespace TicketAPI.Services.DTO;

public class RegisterModelDTO : GeneralRegisterModelDTO
{
    public string Password { get; set; }
    public string RepeatPassword { get; set; }
}
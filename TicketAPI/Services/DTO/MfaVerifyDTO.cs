namespace TicketAPI.Services.DTO;

public class MfaVerifyDTO
{
    public string UserEmail { get; set; }
    public string Code { get; set; }
}
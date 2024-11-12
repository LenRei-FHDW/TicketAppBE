namespace TicketAPI.Services.DTO;

public class ConfirmEmailResultDTO
{
    public bool CompleteRequest { get; set; }
    public bool UserExists { get; set; }
    public bool EmailConfirmed { get; set; }

    public ConfirmEmailResultDTO(bool completeRequest, bool userExists, bool emailConfirmed)
    {
        CompleteRequest = completeRequest;
        UserExists = userExists;
        EmailConfirmed = emailConfirmed;
    }

    public ConfirmEmailResultDTO(bool userExists, bool emailConfirmed)
    {
        UserExists = userExists;
        EmailConfirmed = emailConfirmed;
    }
}
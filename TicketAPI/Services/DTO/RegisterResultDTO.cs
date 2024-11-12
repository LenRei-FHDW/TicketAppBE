namespace TicketAPI.Services.DTO;

public class RegisterResultDTO
{
     public bool SamePassword { get; set; } 
     public bool Succeeded { get; set; }

     public RegisterResultDTO(bool samePassword, bool succeeded)
     {
          SamePassword = samePassword;
          Succeeded = succeeded;
     }
}
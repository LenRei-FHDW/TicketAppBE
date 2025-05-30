namespace TicketAPI.Services.Scoped;

public class GoogleResultDTO
{
    public bool Success { get; set; } = true;
    public string? Message { get; set; }
    public string? Token { get; set; }
}
using Microsoft.Build.Framework;

namespace TicketAPI.Services.DTO;

public class CategoryCreateDTO
{
    [Required]
    public string Name { get; set; }
}
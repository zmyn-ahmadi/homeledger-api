using System.ComponentModel.DataAnnotations;

namespace HomeLedger.API.DTOs.FamilyDTO;

public class CreateInvitationDTO
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
}

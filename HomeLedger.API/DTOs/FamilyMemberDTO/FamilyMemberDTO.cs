namespace HomeLedger.API.DTOs.FamilyMemberDTO;

public class FamilyMemberDTO
{
    public int UserId { get; set; }

    public string? FullName { get; set; }

    public string Email { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;
}

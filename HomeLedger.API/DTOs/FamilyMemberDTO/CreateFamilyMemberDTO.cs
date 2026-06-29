namespace HomeLedger.API.DTOs.FamilyMemberDTO;

public class CreateFamilyMemberDTO
{
    public int UserId { get; set; }

    public string Role { get; set; } = "Member";
}

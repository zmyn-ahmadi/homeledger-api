using HomeLedger.API.DTOs.FamilyMemberDTO;
using HomeLedger.API.DTOs.UserDTO;
using HomeLedger.API.Models;

namespace HomeLedger.API.DTOs.FamilyDTO;

public class FamilyDTO
{
    public int FamilyId { get; set; }

    public string Name { get; set; } = string.Empty;

    public List<FamilyMemberDTO.FamilyMemberDTO> Members { get; set; } = [];
}

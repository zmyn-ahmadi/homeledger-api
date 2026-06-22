using HomeLedger.API.DTOs.UserDTO;
using HomeLedger.API.Models;

namespace HomeLedger.API.DTOs.FamilyDTO;

public class FamilyDTO
{
    public int FamilyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<FamilyUserDTO> Users { get; set; } = [];
    public ICollection<FamilyBillDTO> Bills { get; set; } = [];
    public DateTime CreatedAt { get; set; }
}

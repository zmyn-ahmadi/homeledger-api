using HomeLedger.API.DTOs.UserDTO;
using HomeLedger.API.Models;

namespace HomeLedger.API.DTOs.FamilyDTO;

public class FamilyDTO
{
    public int FamilyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public virtual ICollection<User> Members { get; set; } = [];
    public DateTime CreatedAt { get; set; }
}

using System.ComponentModel.DataAnnotations;

namespace HomeLedger.API.DTOs.FamilyDTO;

public class CreateFamilyDTO
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    public List<int> MemberIds { get; set; } = [];
}


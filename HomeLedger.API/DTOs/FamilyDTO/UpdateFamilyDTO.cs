using System.ComponentModel.DataAnnotations;

namespace HomeLedger.API.DTOs.FamilyDTO;

public class UpdateFamilyDTO
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
}

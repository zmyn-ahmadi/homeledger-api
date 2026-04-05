using System.ComponentModel.DataAnnotations;

namespace HomeLedger.API.DTOs.CategoryDTO;

public class UpdateCategoryDTO
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string Color { get; set; }
    public string? Icon { get; set; }
}

using System.ComponentModel.DataAnnotations;

namespace HomeLedger.API.DTOs.CategoryDTO;

public class UpdateCategoryDTO
{
    public string? Name { get; set; }
    public string? Color { get; set; }
    public string? Icon { get; set; }
}

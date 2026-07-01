using System.ComponentModel.DataAnnotations;
using HomeLedger.API.Models;

namespace HomeLedger.API.DTOs.CategoryDTO;

public class CreateCategoryDTO
{
    public required string Name { get; set; }

    public string? Color { get; set; } = string.Empty;
    public string? Icon { get; set; }
}

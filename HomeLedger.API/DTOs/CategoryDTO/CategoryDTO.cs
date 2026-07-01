using HomeLedger.API.Models;

namespace HomeLedger.API.DTOs.CategoryDTO;

public class CategoryDTO
{
    public int CategoryId { get; set; }

    public string? Name { get; set; }
    public string? Color { get; set; }
    public string? Icon { get; set; }
}

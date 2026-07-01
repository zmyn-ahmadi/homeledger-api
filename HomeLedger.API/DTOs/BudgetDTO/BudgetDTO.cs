using HomeLedger.API.Models;
using CategoryDto = HomeLedger.API.DTOs.CategoryDTO.CategoryDTO;

namespace HomeLedger.API.DTOs.BudgetDTO;

public class BudgetDTO
{
    public int BudgetId { get; set; }

    public string Name { get; set; } = string.Empty;
    public decimal LimitAmount { get; set; }
    public Period Period { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int AlertThreshold { get; set; } = 80;
    public int? CategoryId { get; set; }

    // Embedded category so the client renders name/icon/color without a
    // separate fetch + fragile client-side join by id. Null when uncategorized.
    public CategoryDto? Category { get; set; }
}

using HomeLedger.API.Models;
using CategoryDto = HomeLedger.API.DTOs.CategoryDTO.CategoryDTO;

namespace HomeLedger.API.DTOs.ExpensesDTO;

public class ExpenseDTO
{
    public int ExpenseId { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string Description { get; set; } = null!;
    public string? Note { get; set; }
    public int? CategoryId { get; set; }

    // Embedded category so the client renders name/icon/color without a
    // separate fetch + fragile client-side join by id. Null when uncategorized.
    public CategoryDto? Category { get; set; }
}

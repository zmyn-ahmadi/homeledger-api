using HomeLedger.API.Models;
using CategoryDto = HomeLedger.API.DTOs.CategoryDTO.CategoryDTO;

namespace HomeLedger.API.DTOs.BillDTO;

public class BillDTO : BaseAuditableEntity
{
    public int BillId { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public DateTime DueDate { get; set; }

    public string Status { get; set; } = "NotPaid";
    public bool IsRecurring { get; set; } = false;
    public RecurringFrequency? RecurringFrequency { get; set; }
    public string? Notes { get; set; }
    public string Scope { get; set; } = string.Empty;
    public int? CategoryId { get; set; }

    // Embedded category so the client renders name/icon/color without a
    // separate fetch + fragile client-side join by id. Null when uncategorized.
    public CategoryDto? Category { get; set; }
}

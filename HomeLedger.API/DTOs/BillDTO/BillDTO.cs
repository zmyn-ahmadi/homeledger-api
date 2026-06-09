using HomeLedger.API.Models;

namespace HomeLedger.API.DTOs.BillDTO;

public class BillDTO : BaseAuditableEntity
{
    public int BillId { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public DateTime DueDate { get; set; }

    public BillStatus Status { get; set; } = BillStatus.Upcoming;
    public bool IsRecurring { get; set; } = false;
    public RecurringFrequency? RecurringFrequency { get; set; }
    public string? Notes { get; set; }
    public string Scope { get; set; } = string.Empty;
}

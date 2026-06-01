using HomeLedger.API.Models;

namespace HomeLedger.API.DTOs.BillDTO;

public class UpdateBillDTO
{
    public string? Name { get; set; }

    public decimal Amount { get; set; }

    public DateTime DueDate { get; set; }

    public bool IsPaid { get; set; } = false;
    public bool IsRecurring { get; set; } = false;
    public RecurringFrequency? RecurringFrequency { get; set; }
    public string? Notes { get; set; }
}

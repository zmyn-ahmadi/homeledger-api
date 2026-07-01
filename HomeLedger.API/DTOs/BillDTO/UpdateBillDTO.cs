using HomeLedger.API.Models;

namespace HomeLedger.API.DTOs.BillDTO;

public class UpdateBillDTO
{
    public string? Name { get; set; }

    public decimal Amount { get; set; }

    public DateTime DueDate { get; set; }

    public string Status { get; set; } = "NotPaid";
    public bool IsRecurring { get; set; } = false;
    public RecurringFrequency? RecurringFrequency { get; set; }
    public string? Notes { get; set; }
    public int? CategoryId { get; set; }
}

using HomeLedger.API.Models;

namespace HomeLedger.API.DTOs.IncomeDTO;

public class CreateIncomeDTO
{
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string Description { get; set; } = null!;
    public string? Note { get; set; }

    public bool IsRecurring { get; set; } = false;
    public RecurringFrequency RecurringFrequency { get; set; }
    public int CategoryId { get; set; }
}

using HomeLedger.API.Models;

namespace HomeLedger.API.DTOs.BudgetDTO;

public class BudgetDTO
{
    public int BudgetId { get; set; }

    public decimal LimitAmount { get; set; }
    public Period Period { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int AlertThreshold { get; set; } = 80;
    public int CategoryId { get; set; }
}

using HomeLedger.API.Models;

namespace HomeLedger.API.DTOs.BudgetDTO;

public class UpdateBudgetDTO
{
    public decimal LimitAmount { get; set; }
    public Period Period { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int CategoryId { get; set; }
}

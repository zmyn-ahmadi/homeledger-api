using HomeLedger.API.Models;

namespace HomeLedger.API.DTOs.ExpensesDTO;

public class CreateExpenseDTO
{
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string Description { get; set; } = null!;
    public string? Note { get; set; }
    public int? CategoryId { get; set; }
}

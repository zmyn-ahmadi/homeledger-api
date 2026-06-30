namespace HomeLedger.API.DTOs.FamilyDTO;

public class FamilyExpenseDTO
{
    public int ExpenseId { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string? Description { get; set; }
    public int UserId { get; set; }
    public string? FullName { get; set; }
}

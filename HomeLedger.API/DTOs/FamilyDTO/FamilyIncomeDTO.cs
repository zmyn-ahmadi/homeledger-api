namespace HomeLedger.API.DTOs.FamilyDTO;

public class FamilyIncomeDTO
{
    public int IncomeId { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string? Description { get; set; }
    public string Note { get; set; } = null!;
    public int UserId { get; set; }
    public string? FullName { get; set; }
}

namespace HomeLedger.API.DTOs.ExpensesDTO;

public class ExpenseCategoryTotalDTO
{
    public string Category { get; set; }
    public int CategoryId { get; set; }
    public string Icon { get; set; }

    public decimal Total { get; set; }
    public int Count { get; set; }
}

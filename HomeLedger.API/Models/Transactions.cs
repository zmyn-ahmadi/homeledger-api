using HomeLedger.API.Models;

public class Transaction
{
    public int TransactionId { get; set; }

    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string? Description { get; set; }

    public int AccountId { get; set; }
    public virtual Account? Account { get; set; }
}

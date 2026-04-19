namespace HomeLedger.API.DTOs.TransactionDTO;

public class TransactionDTO
{
    public int TransactionId { get; set; }

    public decimal Amount { get; set; }
    public DateTime Date { get; set; }

    public int AccountId { get; set; }
    public Account Account { get; set; }

    public virtual Category Category { get; set; }
}

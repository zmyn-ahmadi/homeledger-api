namespace HomeLedger.API.Models;

public class Account : BaseAuditableEntity
{
    public int AccountId { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public string Currency { get; set; }
    public decimal AccountBalance { get; set; }

    public int OwnerId { get; set; }
    public virtual User Owner { get; set; }

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}

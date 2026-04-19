using HomeLedger.API.Models;

namespace HomeLedger.API.DTOs.AccountDTO;

public class AccountDTO : BaseAuditableEntity
{
    public string Name { get; set; }
    public string Type { get; set; }
    public string Currency { get; set; }
    public decimal AccountBalance { get; set; }

    public int OwnerId { get; set; }
}

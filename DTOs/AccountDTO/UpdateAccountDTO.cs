namespace HomeLedger.API.DTOs.AccountDTO;

public class UpdateAccountDTO
{
    public required string Name { get; set; }
    public required string Type { get; set; }
    public string? Currency { get; set; }
    public decimal AccountBalance { get; set; }
}

namespace HomeLedger.API.DTOs.LinkDTO;

public class LinkDTO
{
    public int LinkId { get; set; }

    public string Code { get; set; } = string.Empty;
    public bool IsRevoked { get; set; } = false;
    public DateTime? ExpiresAt { get; set; }
    public int MaxUses { get; set; } = 1;
    public int UseCount { get; set; } = 0;

    public int? FamilyId { get; set; }
    public int CreatedBy { get; set; }
}

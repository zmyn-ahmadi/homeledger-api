using System.ComponentModel.DataAnnotations.Schema;

namespace HomeLedger.API.Models;

public class Link : BaseAuditableEntity
{
    public int LinkId { get; set; }

    public string Code { get; set; } = string.Empty;
    public bool IsRevoked { get; set; } = false;
    public DateTime? ExpiresAt { get; set; }
    public int MaxUses { get; set; } = 1;
    public int UseCount { get; set; }

    public int FamilyId { get; set; }
    public int CreatedBy { get; set; }

    public Family Family { get; set; } = null!;
    public User Creator { get; set; } = null!;

    public ICollection<LinkUsage> Usages { get; set; } = [];
}

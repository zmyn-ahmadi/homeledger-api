using System.ComponentModel.DataAnnotations;

namespace HomeLedger.API.Models;

public class FamilyMember : BaseAuditableEntity
{
    public int UserId { get; set; }
    public int FamilyId { get; set; }

    public string Role { get; set; } = "Member";

    public virtual User User { get; set; } = null!;
    public virtual Family Family { get; set; } = null!;
}

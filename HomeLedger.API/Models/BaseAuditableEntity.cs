namespace HomeLedger.API.Models;

public abstract class BaseAuditableEntity
{
  public bool IsActive { get; set; } = true;
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  public DateTime? UpdatedAt { get; set; }
}

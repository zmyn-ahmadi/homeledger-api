using System.ComponentModel.DataAnnotations;

namespace HomeLedger.API.Models;

public class UserProfile : BaseAuditableEntity
{
  [Key]
  public int UserProfileId { get; set; }

  [Phone]
  public string? Phone { get; set; }
  public DateTime DOB { get; set; }
  public string? Address { get; set; }

  // Every profile must belong to a user
  public int UserId { get; set; }
  public virtual User User { get; } = null!;
}

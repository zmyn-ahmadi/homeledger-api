using System.ComponentModel.DataAnnotations;

namespace HomeLedger.API.DTOs.UserProfile;

public class UserProfileDTO
{
  [Key]
  public int UserProfileId { get; set; }

  [Phone]
  public string? Phone { get; set; }
  public DateTime DOB { get; set; }
  public string? Address { get; set; }
}

using System.ComponentModel.DataAnnotations;

namespace HomeLedger.API.DTOs.UserProfile;

public class UserProfileDTO
{
  [Phone]
  public string? Phone { get; set; }
  public DateTime DOB { get; set; }
  public string? Address { get; set; }
}

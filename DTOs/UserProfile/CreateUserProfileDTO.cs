using System.ComponentModel.DataAnnotations;

namespace HomeLedger.API.DTOs.UserProfile;

public class CreateUserProfileDTO
{
  [Required]
  [Phone]
  public string Phone { get; set; }

  [Required]
  public DateTime DOB { get; set; }

  [Required]
  public string Address { get; set; }
}

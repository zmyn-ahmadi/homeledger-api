using System.ComponentModel.DataAnnotations;

namespace HomeLedger.API.Dtos.User;

public class UpdateRequestDto
{
  [Required]
  [EmailAddress]
  public string? Email { get; set; }

  [Required]
  public string? Password { get; set; }
  public string? FirstName { get; set; }
  public string? LastName { get; set; }
  public bool IsSuperUser { get; set; } = false;
}

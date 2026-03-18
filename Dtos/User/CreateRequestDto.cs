using System.ComponentModel.DataAnnotations;

namespace HomeLedger.API.Dtos.User;

public class CreateRequestDto
{
  [Required]
  [MaxLength(150, ErrorMessage = "Username must be 150 characters or less.")]
  public string? UserName { get; set; }

  [Required]
  [EmailAddress]
  public string? Email { get; set; }

  [Required]
  public string? Password { get; set; }

  public string? FirstName { get; set; }
  public string? LastName { get; set; }
  public DateTime DateJoined { get; } = DateTime.Now;
  public bool IsSuperUser { get; set; } = false;
}

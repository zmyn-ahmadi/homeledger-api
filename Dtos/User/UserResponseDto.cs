using System.ComponentModel.DataAnnotations;

namespace HomeLedger.API.Dtos.User;

public class UserResponseDto
{
  public int Id { get; set; }

  [Required]
  [MaxLength(150, ErrorMessage = "Username must be 150 characters or less.")]
  public string? UserName { get; set; }

  [Required]
  [EmailAddress]
  public string? Email { get; set; }

  public string? FirstName { get; set; }
  public string? LastName { get; set; }
  public DateTime DateJoined { get; } = DateTime.Now;
}

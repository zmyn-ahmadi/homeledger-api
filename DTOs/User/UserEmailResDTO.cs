using System.ComponentModel.DataAnnotations;

namespace HomeLedger.API.DTOs.User;

public class UserEmailResDTO
{
  public int UserId { get; set; }

  [MaxLength(150, ErrorMessage = "Username must be 150 characters or less.")]
  public string UserName { get; set; }

  [EmailAddress]
  public string Email { get; set; }

  public string FullName { get; set; }
}

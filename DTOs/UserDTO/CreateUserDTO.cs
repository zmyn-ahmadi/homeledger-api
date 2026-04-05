using System.ComponentModel.DataAnnotations;
using HomeLedger.API.DTOs.UserProfile;

namespace HomeLedger.API.DTOs.UserDTO;

public class CreateUserDTO
{
    [Required]
    [MaxLength(150, ErrorMessage = "Username must be 150 characters or less.")]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    public UserProfileDTO? Profile { get; set; }
}

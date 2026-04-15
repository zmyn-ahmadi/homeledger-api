using System.ComponentModel.DataAnnotations;
using HomeLedger.API.DTOs.UserProfile;

namespace HomeLedger.API.DTOs.UserDTO;

public class CreateUserDTO
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    public string? FullName { get; set; }

    public UserProfileDTO? Profile { get; set; }
}

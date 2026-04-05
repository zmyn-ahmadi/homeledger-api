using System.ComponentModel.DataAnnotations;

namespace HomeLedger.API.DTOs.UserDTO;

public class UpdateUserDTO
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }
}

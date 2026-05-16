using System.ComponentModel.DataAnnotations;

namespace HomeLedger.API.DTOs.UserDTO;

public class UserEmailResDTO
{
    public int UserId { get; set; }

    [EmailAddress]
    public string Email { get; set; }

    public string FullName { get; set; }
}

using HomeLedger.API.DTOs.UserDTO;

namespace HomeLedger.API.Utils.UserContext;

public interface IUserContext
{
    int UserId { get; }
    UserDTO User { get; }
}

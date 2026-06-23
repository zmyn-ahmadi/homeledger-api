using System.Security.Claims;
using HomeLedger.API.DTOs.UserDTO;

namespace HomeLedger.API.Utils.UserContext;

public class UserContext(IHttpContextAccessor accessor) : IUserContext
{
    private readonly IHttpContextAccessor _accessor = accessor;

    public int UserId
    {
        get
        {
            var claim = _accessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(claim, out var id))
                throw new UnauthorizedAccessException("No authenticated user.");
            return id;
        }
    }

    public UserDTO User
    {
        get
        {
            var claims =
                (_accessor.HttpContext?.User?.Claims)
                ?? throw new UnauthorizedAccessException("No authenticated user.");

            var user = new UserDTO
            {
                UserId = int.Parse(
                    claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? "0"
                ),
                Email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value,
                FullName = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value,
            };

            return user;
        }
    }

    // new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
    // new Claim(ClaimTypes.Email, user.Email!),
    // new Claim(ClaimTypes.Name, user.FullName!),
    // new Claim("IsSuperUser", user.IsSuperUser.ToString()),

    // public User GetUser
    // {
    //     get
    //     {
    //         var claims = _accessor.HttpContext?.User?.Claims;
    //         if (claims == null)
    //             throw new UnauthorizedAccessException("No authenticated user.");

    //         var user = new User
    //         {
    //             UserId = int.Parse(claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? "0"),
    //             Email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value,
    //             FullName = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value,
    //         };

    //         return user;

    //     }
    // }
}

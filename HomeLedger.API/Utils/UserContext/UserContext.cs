using System.Security.Claims;

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
}

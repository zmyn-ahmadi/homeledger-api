using System.Security.Claims;
using HomeLedger.API.Services.Basiq;
using HomeLedger.API.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeLedger.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BasiqController(BasiqServices basiq, IUserServices userService) : ControllerBase
{
    private readonly BasiqServices _basiq = basiq;
    private readonly IUserServices _user = userService;

    [HttpPost("connect")]
    public async Task<IActionResult> Connect()
    {
        // Get current user from JWT
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user = await _user.GetUserAsync(userId);
        if (user == null)
            return Unauthorized();

        // creat Basiq user if not already linked
        return NoContent();
    }
}

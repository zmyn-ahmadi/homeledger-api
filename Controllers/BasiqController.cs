using System.Security.Claims;
using HomeLedger.API.Persistence;
using HomeLedger.API.Services.Basiq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeLedger.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BasiqController(BasiqServices basiq, HomeLedgerDbContext db) : ControllerBase
{
    [HttpPost("connect")]
    public async Task<IActionResult> Connect()
    {
        // Get current user from JWT
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user = await db.Users.FindAsync(userId);
        if (user == null)
            return Unauthorized();

        // check if basiqUser exists
        if (!await basiq.CheckUserExistAsync(user.BasiqUserId!))
        {
            // delete the basiqUserId in the database
            user.BasiqUserId = "";
        }

        // creat Basiq user if not already linked
        if (string.IsNullOrEmpty(user.BasiqUserId))
        {
            user.BasiqUserId = await basiq.CreateBasiqUserAsync(
                user.Email,
                user.Profile.Phone ?? ""
            );
            await db.SaveChangesAsync();
        }

        // Get the content/auth URL
        var consentUrl = await basiq.GetConsentUrlAsync(user.BasiqUserId);
        return Ok(new { consentUrl });
    }
}

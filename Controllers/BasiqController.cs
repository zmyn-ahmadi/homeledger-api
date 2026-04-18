using System.Security.Claims;
using HomeLedger.API.Services.Basiq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeLedger.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BasiqController(BasiqServices basiqService) : ControllerBase
{
    private readonly BasiqServices _basiq = basiqService;

    [HttpPost("connect")]
    public async Task<IActionResult> Connect()
    {
        // Get current user from JWT
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var consentUrl = await _basiq.ConnectUserAsync(userId);

        return Ok(new { consentUrl });
    }

    [HttpGet("accounts")]
    public async Task<IActionResult> GetAccounts()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var accounts = await _basiq.GetAccountsAsync(userId);

        return Ok(new { accounts });
    }

    [HttpGet("transactions")]
    public async Task<IActionResult> GetTransactions()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var transactions = await _basiq.GetTransactionsAsync(userId);

        return Ok(new { transactions });
    }

    [HttpGet("connection-status")]
    public async Task<IActionResult> GetConnectionStatus()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var status = await _basiq.GetConnectionStatusAsync(userId);

        return Ok(status);
    }
}

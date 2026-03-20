using HomeLedger.API.Dtos;
using HomeLedger.API.Persistence;
using HomeLedger.API.Services.Token;
using HomeLedger.API.Services.Users;
using HomeLedger.API.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HomeLedger.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
public class TokenController(
    IUserServices userService,
    JwtService jwtService,
    HomeLedgerDbContext db
) : ControllerBase
{
    private readonly IUserServices _userServices = userService;
    private readonly JwtService _jwtService = jwtService;
    private readonly HomeLedgerDbContext _db = db;

    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginRequest request)
    {
        if (request.Email == null)
        {
            return NotFound("Email is required");
        }
        var user = await _userServices.GetUserByEmailAsync(request.Email);
        if (user == null || !PasswordManager.Verify(request.Password, user.Password))
            return Unauthorized("Invalid credentials");

        // update last login
        user.LastLogin = DateTime.UtcNow;

        // generate tokens
        var (accessToken, refreshToken) = _jwtService.GenerateTokens(user);

        // save that to db
        await _db.RefreshTokens.AddAsync(refreshToken);
        await _db.SaveChangesAsync();

        // Return it in HttpOnly cookie
        Response.Cookies.Append(
            "refreshToken",
            refreshToken.Token,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = refreshToken.Expiry,
            }
        );

        return Ok(new { accessToken });
    }

    [HttpPost("refresh")]
    public async Task<ActionResult> RefreshToken()
    {
        var incomingToken = Request.Cookies["refreshToken"];
        if (incomingToken == null)
            return Unauthorized("No refresh token found.");

        // Find the refresh token in db
        var storedToken = await _db
            .RefreshTokens.Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Token == incomingToken);

        if (storedToken == null || storedToken.IsRevoked || storedToken.Expiry < DateTime.UtcNow)
            return Unauthorized("Invalid or expired refresh token");

        // Rotate: revoke old, issue new
        storedToken.IsRevoked = true;
        var (newAccessToken, newRefreshToken) = _jwtService.GenerateTokens(storedToken.User);
        await _db.RefreshTokens.AddAsync(newRefreshToken);
        await _db.SaveChangesAsync();

        Response.Cookies.Append(
            "refreshToken",
            newRefreshToken.Token,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = newRefreshToken.Expiry,
            }
        );

        return Ok(new { accessToken = newAccessToken });
    }
}

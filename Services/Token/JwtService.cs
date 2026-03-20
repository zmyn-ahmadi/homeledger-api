using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using HomeLedger.API.Models;
using Microsoft.IdentityModel.Tokens;

namespace HomeLedger.API.Services.Token;

public class JwtService(IConfiguration config)
{
  private readonly IConfiguration _config = config;

  public (string AccessToken, RefreshToken RefreshToken) GenerateTokens(User user)
  {
    var accessToken = GenerateAccessToken(user);
    var refreshToken = GenerateRefreshToken(user);

    return (accessToken, refreshToken);
  }

  private string GenerateAccessToken(User user)
  {
    var claims = new[]
    {
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim("IsSuperUser", user.IsSuperUser.ToString()),
        };

    var keyString =
        _config["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is not configured");

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));

    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: _config["Jwt:Issuer"],
        audience: _config["Jwt:Audience"],
        claims: claims,
        expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["Jwt:DurationInMinutes"])),
        signingCredentials: creds
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
  }

  private RefreshToken GenerateRefreshToken(User user)
  {
    // save the refresh token into db
    // Users
    // --------------------------------------
    // id | Email | RefreshToken | Expiry
    // --------------------------------------
    // 1  | a@g.com | kx92....   | 2026-04-10
    var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    return new RefreshToken
    {
      Token = token,
      Created = DateTime.UtcNow,
      Expiry = DateTime.UtcNow.AddDays(7),
      UserId = user.UserId,
      User = user,
      IsRevoked = false,
    };
  }
}

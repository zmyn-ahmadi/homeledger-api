namespace HomeLedger.API.Models;

public class TokenResult
{
  public string AccessToken { get; set; } = string.Empty;
  public RefreshToken RefreshToken { get; set; }
}

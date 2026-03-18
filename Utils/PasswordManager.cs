namespace HomeLedger.API.Utils;

public class PasswordManager
{
  public static string Hash(string password)
  {
    return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
  }

  public static bool Verify(string enteredPassword, string storedHash)
  {
    return BCrypt.Net.BCrypt.Verify(enteredPassword, storedHash);
  }
}

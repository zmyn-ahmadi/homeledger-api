using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeLedger.API.Models;

public class RefreshToken
{
  [Key]
  public int Id { get; set; }

  [Required]
  public string Token { get; set; } = string.Empty;

  [Required]
  public DateTime Expiry { get; set; }
  public DateTime Created { get; set; }

  public bool IsRevoked { get; set; }

  [ForeignKey("UserId")]
  public int UserId { get; set; }
  public virtual User User { get; set; } = null!;
}

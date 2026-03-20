using System.ComponentModel.DataAnnotations;

namespace HomeLedger.API.Models;

public enum Period
{
  Weekly,
  Monthly,
  Yearly,
}

public class Budget : BaseAuditableEntity
{
  [Key]
  public int BudgetId { get; set; }

  [Required]
  [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be between 2 and 10")]
  public decimal Amount { get; set; }

  public Period Period { get; set; }

  [Required]
  public DateTime StartDate { get; set; }

  [Required]
  public DateTime EndDate { get; set; }

  [Range(0, 100, ErrorMessage = "Threshold must be between 0 to 100.")]
  public int AlertThreshold { get; set; } = 80;

  public int OwnerId { get; set; }
  public virtual User Owner { get; set; } = null!;

  public int CategoryId { get; set; }
  public virtual Category Category { get; set; } = null!;
}

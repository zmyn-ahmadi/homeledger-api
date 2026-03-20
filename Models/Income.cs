using System.ComponentModel.DataAnnotations;

namespace HomeLedger.API.Models;

public class Income : BaseAuditableEntity
{
  [Key]
  public int IncomeId { get; set; }

  [Required]
  [MaxLength(200, ErrorMessage = "Source must be 200 characters or less.")]
  public string? Source { get; set; }

  [Required]
  [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
  public decimal Amount { get; set; }

  [Required]
  public DateTime Date { get; set; }
  public string? Description { get; set; }
  public bool IsRecurring { get; set; } = false;
  public RecurringFrequency RecurringFrequency { get; set; }

  // Relationships

  // M:1
  public int OwnerId { get; set; }
  public virtual User Owner { get; set; } = null!;

  public int CategoryId { get; set; }
  public virtual Category Category { get; set; } = null!;
}

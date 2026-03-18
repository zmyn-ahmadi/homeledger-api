using System.ComponentModel.DataAnnotations;

namespace HomeLedger.API.Models;

public class Bill : BaseAuditableEntity
{
  [Key]
  public int BillId { get; set; }

  [Required]
  [MaxLength(200, ErrorMessage = "Bill name must bet 200 characters or less.")]
  public string? Name { get; set; }

  [Required]
  [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
  public decimal Amount { get; set; }

  [Required]
  public DateTime DueDate { get; set; }

  public bool IsPaid { get; set; } = false;
  public bool IsRecurring { get; set; } = false;
  public RecurringFrequency? RecurringFrequency { get; set; }
  public string? Notes { get; set; }

  // many-to-many
  public virtual ICollection<User>? SharedWith { get; set; }

  public int OwnerId { get; set; }
  public virtual User Owner { get; set; } = null!;

  public int CategoryId { get; set; }
  public virtual Category Category { get; set; } = null!;

  public virtual ICollection<BillPayment> BillPayments { get; set; } = new HashSet<BillPayment>();
}

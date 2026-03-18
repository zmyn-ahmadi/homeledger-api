using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeLedger.API.Models;

public class BillPayment : BaseAuditableEntity
{
  [Key]
  public int BillPaymentId { get; set; }

  [Required]
  [Range(2, 10, ErrorMessage = "Amount must be between 2 and 10")]
  [MinLength(0)]
  public decimal Amount { get; set; }

  [Required]
  public DateTime PaidDate { get; set; }

  public string? Notes { get; set; }

  public int BillId { get; set; }
  public virtual Bill Bill { get; set; } = null!;

  public int PaidById { get; set; }
  public virtual User PaidBy { get; set; } = null!;
}

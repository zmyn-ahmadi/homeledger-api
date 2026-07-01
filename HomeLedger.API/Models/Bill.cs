using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeLedger.API.Models;

public class Bill : BaseAuditableEntity
{
    [Key]
    public int BillId { get; set; }

    [Required]
    [MaxLength(200, ErrorMessage = "Bill name must be 200 characters or less.")]
    public string? Name { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
    public decimal Amount { get; set; }

    [Required]
    public DateTime DueDate { get; set; }

    public bool IsRecurring { get; set; } = false;
    public RecurringFrequency? RecurringFrequency { get; set; } = Models.RecurringFrequency.Monthly;

    public string? Notes { get; set; }
    public string Scope { get; set; } = string.Empty;
    public string Status { get; set; } = "NotPaid";

    public int? OwnerId { get; set; }
    public int? FamilyId { get; set; }

    public virtual Family? Family { get; set; }
    public virtual User? Owner { get; set; }

    // Optional category. Nullable so a bill survives its category being
    // deleted/truncated (FK is ON DELETE SET NULL) instead of dangling or cascading.
    public int? CategoryId { get; set; }
    public virtual Category? Category { get; set; }

    public virtual ICollection<BillPayment> BillPayments { get; set; } = [];
}

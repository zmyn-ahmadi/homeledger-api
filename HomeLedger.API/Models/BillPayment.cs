using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeLedger.API.Models;

public class BillPayment : BaseAuditableEntity
{
    [Key]
    public int BillPaymentId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be between 2 and 10")]
    public decimal Amount { get; set; }

    [Required]
    public DateTime PaidDate { get; set; }

    public string? Notes { get; set; }

    public int BillId { get; set; }
    public virtual Bill Bill { get; set; } = null!;

    public int PaidById { get; set; }
    public virtual User PaidBy { get; set; } = null!;
    public string SourceHash { get; set; } = null!;

    public int FamilyId { get; set; }

    [ForeignKey(nameof(FamilyId))]
    public virtual Family? Family { get; set; }
}

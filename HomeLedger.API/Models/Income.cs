using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeLedger.API.Models;

public class Income : BaseAuditableEntity
{
    [Key]
    public int IncomeId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
    public decimal Amount { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [Required]
    public required string Description { get; set; }
    public string Note { get; set; } = null!;

    public bool IsRecurring { get; set; } = false;
    public RecurringFrequency RecurringFrequency { get; set; }

    // Relationships

    // M:1
    public int OwnerId { get; set; }
    public virtual User Owner { get; set; } = null!;

    public int? FamilyId { get; set; }

    [ForeignKey(nameof(FamilyId))]
    public virtual Family? Family { get; set; }
}

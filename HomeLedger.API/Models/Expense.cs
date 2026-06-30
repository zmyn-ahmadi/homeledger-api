using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeLedger.API.Models;

public class Expense : BaseAuditableEntity
{
    [Key]
    public int ExpenseId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
    public decimal Amount { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [Required]
    public required string Description { get; set; }
    public string Note { get; set; } = null!;

    public int OwnerId { get; set; }
    public int CategoryId { get; set; }

    public virtual User Owner { get; set; } = null!;
    public virtual Category Category { get; set; } = null!;
}

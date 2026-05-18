using System.ComponentModel.DataAnnotations;

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

    public string? Description { get; set; }
    public int OwnerId { get; set; }
    public virtual User Owner { get; set; } = null!;
    public int CategoryId { get; set; }
    public virtual Category Category { get; set; } = null!;
}

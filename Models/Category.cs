using System.ComponentModel.DataAnnotations;

namespace HomeLedger.API.Models;

public enum TransactionType
{
  Expense,
  Income,
  Both,
};

public class Category : BaseAuditableEntity
{
  [Key]
  public int CategoryId { get; set; }

  [Required]
  [MaxLength(100, ErrorMessage = "Category name must be 100 characters or less.")]
  public string? Name { get; set; }

  [MaxLength(7)]
  public string? Color { get; set; } = "#6366f1";

  public string? Icon { get; set; }
  public TransactionType Type { get; set; }

  // a category belongs to a user
  public int UserId { get; set; }
  public virtual User Owner { get; set; } = null!;

  // 1:M -> one Category can have many expense
  public virtual ICollection<Expense> Expenses { get; set; } = new HashSet<Expense>();
  public virtual ICollection<Income> Incomes { get; set; } = new HashSet<Income>();
  public virtual ICollection<Budget> Budgets { get; set; } = new HashSet<Budget>();
  public virtual ICollection<Bill> Bills { get; set; } = new HashSet<Bill>();
}

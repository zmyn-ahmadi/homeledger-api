using System.ComponentModel.DataAnnotations;

namespace HomeLedger.API.Models;

public class User
{
    [Key]
    public int UserId { get; set; }

    [Required]
    [MaxLength(150, ErrorMessage = "Username must be 150 characters or less.")]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string? Email { get; set; }

    [Required]
    public string? Password { get; set; }

    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    public DateTime LastLogin { get; set; }
    public bool IsSuperUser { get; set; }

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = [];

    // 1:1 -> a user may have one profile
    public virtual UserProfile? Profile { get; set; }

    // 1:M User can create multiple categories
    public virtual ICollection<Category> Categories { get; set; } = [];
    public virtual ICollection<Expense> Expenses { get; set; } = [];
    public virtual ICollection<Income> Incomes { get; set; } = [];
    public virtual ICollection<Budget> Budgets { get; set; } = [];
    public virtual ICollection<Bill> OwnedBills { get; set; } = [];
    public virtual ICollection<Bill> SharedBills { get; set; } = [];
    public virtual ICollection<BillPayment> BillPayments { get; set; } = [];
}

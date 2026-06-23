using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeLedger.API.Models;

public class User : BaseAuditableEntity
{
    [Key]
    public int UserId { get; set; }

    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    public required string Password { get; set; }

    public string? FullName { get; set; }

    public string? BasiqUserId { get; set; }

    public int? FamilyId { get; set; }

    [ForeignKey(nameof(FamilyId))]
    public virtual Family? Family { get; set; }

    public DateTime LastLogin { get; set; }
    public bool IsSuperUser { get; set; }

    public int AccountId { get; set; }
    public virtual ICollection<Account> Accounts { get; set; } = [];

    // 1:1 -> a user may have one profile
    public virtual UserProfile? Profile { get; set; }
    public virtual WorkDays? WorkDays { get; set; }

    // 1:M User can create multiple categories
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = [];
    public virtual ICollection<Category> Categories { get; set; } = [];
    public virtual ICollection<Expense> Expenses { get; set; } = [];
    public virtual ICollection<Income> Incomes { get; set; } = [];
    public virtual ICollection<Budget> Budgets { get; set; } = [];
    public virtual ICollection<Bill> OwnedBills { get; set; } = [];
    public virtual ICollection<BillPayment> BillPayments { get; set; } = [];
    public virtual ICollection<Link> CreatedInviteLinks { get; set; } = [];
    public virtual ICollection<LinkUsage> LinkUsages { get; set; } = [];
}

using System.ComponentModel.DataAnnotations;

namespace HomeLedger.API.Models;

public class Family : BaseAuditableEntity
{
    [Key]
    public int FamilyId { get; set; }
    public string Name { get; set; } = string.Empty;

    public virtual ICollection<FamilyMember> Members { get; set; } = [];

    public virtual ICollection<Link> InviteLinks { get; set; } = [];
    public virtual ICollection<Bill> Bills { get; set; } = [];
    public virtual ICollection<BillPayment> Payments { get; set; } = [];
    public virtual ICollection<Expense> Expenses { get; set; } = [];
    public virtual ICollection<Income> Incomes { get; set; } = [];
    public virtual ICollection<Budget> Budgets { get; set; } = [];
}

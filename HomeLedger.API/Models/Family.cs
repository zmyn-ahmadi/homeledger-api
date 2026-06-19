using System.ComponentModel.DataAnnotations;

namespace HomeLedger.API.Models;

public class Family
{
    [Key]
    public int FamilyId { get; set; }

    // A family has multiple users
    public virtual ICollection<User> Users { get; set; }

    // A family has bills
    public virtual ICollection<Bill> Bills { get; set; }

    // A family has billPayment
    public virtual ICollection<BillPayment> Payments { get; set; }

    // A family has expenses
    public virtual ICollection<Expense> Expenses { get; set; }

    // A family has incomes
    public virtual ICollection<Income> Incomes { get; set; }

    // A family has budgets
    public virtual ICollection<Budget> Budgets { get; set; }
}

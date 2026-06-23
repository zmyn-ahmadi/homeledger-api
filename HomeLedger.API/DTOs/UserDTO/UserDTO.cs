using System.ComponentModel.DataAnnotations;
using HomeLedger.API.DTOs.UserProfile;

namespace HomeLedger.API.DTOs.UserDTO;

public class UserDTO
{
    public int UserId { get; set; }

    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public int? FamilyId { get; set; }

    // 1:1 -> a user may have one profile
    public virtual UserProfileDTO? Profile { get; set; }

    // 1:M User can create multiple categories
    // public virtual ICollection<CategoryDTO> Categories { get; set; } = [];
    // public virtual ICollection<Expense> Expenses { get; set; } = [];
    // public virtual ICollection<Income> Incomes { get; set; } = [];
    // public virtual ICollection<Budget> Budgets { get; set; } = [];
    // public virtual ICollection<Bill> OwnedBills { get; set; } = [];
    // public virtual ICollection<Bill> SharedBills { get; set; } = [];
    // public virtual ICollection<BillPayment> BillPayments { get; set; } = [];
}

using HomeLedger.API.DTOs.ExpensesDTO;
using HomeLedger.API.DTOs.FamilyDTO;
using HomeLedger.API.Models;
using HomeLedger.API.Persistence.Repositories.Interfaces;

public interface IExpenseRepository : IRepository<Expense>
{
    Task<List<ExpenseCategoryTotalDTO>> GetTotalByCategoryAsync(int userId);
    Task<List<Expense>> GetAllByUserId(int userId);

    // Member's expense
    Task<List<FamilyExpenseDTO>> GetMemberExpensesAsync(int familyId);
}

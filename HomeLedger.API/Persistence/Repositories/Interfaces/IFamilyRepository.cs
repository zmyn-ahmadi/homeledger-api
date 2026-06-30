using HomeLedger.API.Models;

namespace HomeLedger.API.Persistence.Repositories.Interfaces;

public interface IFamilyRepository : IRepository<Family>
{
    Task<List<Bill>> GetFamilyBillsAsync(int familyId);
    Task<Bill?> GetFamilyBillAsync(int familyId, int billId);
    Task<List<Budget>> GetFamilyBudgetsAsync(int familyId);
    Task<Budget?> GetFamilyBudgetAsync(int familyId, int budgetId);
    Task<List<Expense>> GetFamilyExpensesAsync(int familyId);
    Task<List<Income>> GetFamilyIncomesAsync(int familyId);
}

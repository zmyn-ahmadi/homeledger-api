using HomeLedger.API.DTOs.ExpensesDTO;
using HomeLedger.API.DTOs.IncomeDTO;
using HomeLedger.API.Models;
using HomeLedger.API.Persistence;
using HomeLedger.API.Persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HomeLedger.API.Persistence.Repositories;

public class FamilyRepository(HomeLedgerDbContext context)
    : Repository<Family>(context),
        IFamilyRepository
{
    public override async Task<IEnumerable<Family>> GetAllAsync() =>
        await _dbSet.Include(f => f.Members).ThenInclude(m => m.User).ToListAsync();

    public override async Task<Family?> GetByIdAsync(int id) =>
        await _dbSet
            .Include(f => f.Members)
                .ThenInclude(m => m.User)
            .FirstOrDefaultAsync(f => f.FamilyId == id);

    public async Task<List<Bill>> GetFamilyBillsAsync(int familyId) =>
        await _dbSet.Where(f => f.FamilyId == familyId).SelectMany(f => f.Bills).ToListAsync();

    public async Task<List<Budget>> GetFamilyBudgetsAsync(int familyId) =>
        await _dbSet.Where(f => f.FamilyId == familyId).SelectMany(f => f.Budgets).ToListAsync();

    public async Task<List<Expense>> GetFamilyExpensesAsync(int familyId) =>
        await _dbSet.Where(f => f.FamilyId == familyId).SelectMany(f => f.Expenses).ToListAsync();

    public async Task<List<Income>> GetFamilyIncomesAsync(int familyId) =>
        await _dbSet.Where(f => f.FamilyId == familyId).SelectMany(f => f.Incomes).ToListAsync();

    public async Task<Bill?> GetFamilyBillAsync(int familyId, int billId)
    {
        var family = await _dbSet
            .Include(f => f.Bills)
            .FirstOrDefaultAsync(f => f.FamilyId == familyId);

        return family?.Bills.FirstOrDefault(b => b.BillId == billId);
    }

    public async Task<Budget?> GetFamilyBudgetAsync(int familyId, int budgetId)
    {
        var family = await _dbSet
            .Include(f => f.Budgets)
            .FirstOrDefaultAsync(f => f.FamilyId == familyId);

        return family?.Budgets.FirstOrDefault(b => b.BudgetId == budgetId);
    }
}

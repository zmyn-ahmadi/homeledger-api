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
}

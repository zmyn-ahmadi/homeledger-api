using HomeLedger.API.DTOs.FamilyDTO;
using HomeLedger.API.Models;
using HomeLedger.API.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HomeLedger.API.Persistence.Repositories;

public class FamilyMemberRepository(HomeLedgerDbContext context)
    : Repository<FamilyMember>(context),
        IFamilyMemberRepository
{
    public async Task<FamilyMember?> GetByUserIdAndFamilyIdAsync(int userId, int familyId)
    {
        return await _dbSet.FirstOrDefaultAsync(x => x.UserId == userId && x.FamilyId == familyId);
    }

    public async Task<FamilyMember?> GetByUserIdAsync(int userId)
    {
        return await _dbSet.FirstOrDefaultAsync(x => x.UserId == userId);
    }

    public async Task<List<FamilyExpenseDTO>> GetMemberExpensesAsync(int familyId)
    {
        return await _dbSet
            .Where(fm => fm.FamilyId == familyId)
            .SelectMany(fm => fm.User.Expenses)
            .OrderByDescending(e => e.Date)
            .Select(e => new FamilyExpenseDTO
            {
                ExpenseId = e.ExpenseId,
                Amount = e.Amount,
                Date = e.Date,
                Description = e.Description,
                UserId = e.OwnerId,
                FullName = e.Owner != null ? e.Owner.FullName : null,
            })
            .ToListAsync();
    }

    public async Task<List<FamilyIncomeDTO>> GetMemberIncomesAsync(int familyId)
    {
        return await _dbSet
            .Where(fm => fm.FamilyId == familyId)
            .SelectMany(fm => fm.User.Incomes)
            .OrderByDescending(i => i.Date)
            .Select(i => new FamilyIncomeDTO
            {
                IncomeId = i.IncomeId,
                Amount = i.Amount,
                Date = i.Date,
                Description = i.Description,
                Note = i.Note,
                UserId = i.OwnerId,
                FullName = i.Owner != null ? i.Owner.FullName : null,
            })
            .ToListAsync();
    }
}

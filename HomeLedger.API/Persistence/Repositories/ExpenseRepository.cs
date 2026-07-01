using HomeLedger.API.DTOs.ExpensesDTO;
using HomeLedger.API.DTOs.FamilyDTO;
using HomeLedger.API.Models;
using HomeLedger.API.Persistence;
using Microsoft.EntityFrameworkCore;

public class ExpenseRepository(HomeLedgerDbContext context)
    : Repository<Expense>(context),
        IExpenseRepository
{
    // Sum expenses per category for a user. Expenses whose category was removed
    // (CategoryId == null) are grouped into a single "uncategorized" bucket.
    public async Task<List<ExpenseCategoryTotalDTO>> GetTotalByCategoryAsync(int userId)
    {
        var expensesByCategory = await _dbSet
            .Where(e => e.OwnerId == userId && e.IsActive)
            .GroupBy(e => new
            {
                e.CategoryId,
                Name = e.Category != null ? e.Category.Name : null,
                Icon = e.Category != null ? e.Category.Icon : null,
            })
            .Select(g => new ExpenseCategoryTotalDTO
            {
                CategoryId = g.Key.CategoryId ?? 0,
                Category = g.Key.Name ?? "uncategorized",
                Icon = g.Key.Icon ?? "more-horizontal",
                Total = g.Sum(e => e.Amount),
                Count = g.Count(),
            })
            .ToListAsync();

        return expensesByCategory;
    }

    public async Task<List<Expense>> GetAllByUserId(int userId) =>
        await _dbSet
            .Include(e => e.Category)
            .Where(e => e.OwnerId == userId)
            .ToListAsync();

    public async Task<List<FamilyExpenseDTO>> GetMemberExpensesAsync(int familyId)
    {
        return await _dbSet
            .Where(e =>
                e.Owner != null && e.Owner.FamilyMemberships.Any(fm => fm.FamilyId == familyId)
            )
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
}

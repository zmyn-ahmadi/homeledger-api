using HomeLedger.API.DTOs.ExpensesDTO;
using HomeLedger.API.DTOs.FamilyDTO;
using HomeLedger.API.Models;
using HomeLedger.API.Persistence;
using Microsoft.EntityFrameworkCore;

public class ExpenseRepository(HomeLedgerDbContext context)
    : Repository<Expense>(context),
        IExpenseRepository
{
    // public async Task<List<ExpenseCategoryTotalDTO>> GetTotalByCategoryAsync(int userId)
    // {
    //     var expensesByCategory = await _context
    //         .Expenses.Where(e => e.OwnerId == userId && e.IsActive)
    //         .GroupBy(e => e.Category.Name)
    //         .Select(g => new ExpenseCategoryTotalDTO
    //         {
    //             Category = g.Key,
    //             CategoryId = g.First().CategoryId,
    //             Icon = g.First().Category.Icon,
    //
    //             Total = g.Sum(e => e.Amount),
    //             Count = g.Count(),
    //         })
    //         .ToListAsync();
    //
    //     return expensesByCategory;
    // }

    public async Task<List<Expense>> GetAllByUserId(int userId) =>
        await _dbSet.Where(e => e.OwnerId == userId).ToListAsync();

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

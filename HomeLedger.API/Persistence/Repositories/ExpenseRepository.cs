using HomeLedger.API.DTOs.ExpensesDTO;
using HomeLedger.API.Models;
using HomeLedger.API.Persistence;
using Microsoft.EntityFrameworkCore;

public class ExpenseRepository(HomeLedgerDbContext context)
    : Repository<Expense>(context),
        IExpenseRepository
{
    public async Task<List<ExpenseCategoryTotalDTO>> GetTotalByCategoryAsync(int userId)
    {
        var expensesByCategory = await _context
            .Expenses.Where(e => e.OwnerId == userId && e.IsActive)
            .GroupBy(e => e.Category.Name)
            .Select(g => new ExpenseCategoryTotalDTO
            {
                Category = g.Key,
                Total = g.Sum(e => e.Amount),
                Count = g.Count(),
                Icon = g.First().Category.Icon,
            })
            .ToListAsync();

        return expensesByCategory;
    }
}

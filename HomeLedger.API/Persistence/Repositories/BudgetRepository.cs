using HomeLedger.API.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeLedger.API.Persistence.Repositories;

public class BudgetRepository(HomeLedgerDbContext context)
    : Repository<Budget>(context),
        IBudgetRepository
{
    public async Task<List<Budget>> GetAllByUserIdAsync(int userId) =>
        await _dbSet
            .Include(b => b.Category)
            .Where(b => b.OwnerId == userId)
            .ToListAsync();
}

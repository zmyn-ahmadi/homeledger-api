using HomeLedger.API.Models;
using HomeLedger.API.Persistence;
using Microsoft.EntityFrameworkCore;

public class IncomeRepository(HomeLedgerDbContext context)
    : Repository<Income>(context),
        IIncomeRepository
{
    public async Task<List<Income>> GetAllByUserIdAsync(int userId) =>
        await _dbSet.Where(i => i.OwnerId == userId).ToListAsync();
}

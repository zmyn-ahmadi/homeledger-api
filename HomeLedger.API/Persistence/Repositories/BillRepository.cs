using HomeLedger.API.Models;
using HomeLedger.API.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HomeLedger.API.Persistence.Repositories;

public class BillRepository(HomeLedgerDbContext context)
    : Repository<Bill>(context),
        IBillRepository
{
    public async Task<List<Bill>> GetAllByUserIdAsync(int userId) =>
        await _dbSet.Where(b => b.OwnerId == userId).ToListAsync();
}

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
        await _dbSet.Include(u => u.Users).ToListAsync();

    public override async Task<Family?> GetByIdAsync(int id) =>
        await _dbSet.Include(u => u.Users).FirstOrDefaultAsync(f => f.FamilyId == id);
}

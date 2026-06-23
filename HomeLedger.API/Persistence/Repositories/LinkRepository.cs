using HomeLedger.API.Models;
using HomeLedger.API.Persistence;
using HomeLedger.API.Persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HomeLedger.API.Persistence.Repositories;

public class LinkRepository(HomeLedgerDbContext context)
    : Repository<Link>(context),
        ILinkRepository
{
    public async Task<Link?> GetLinkByCode(string code) =>
        await _dbSet
            .Include(l => l.Family)
            .Include(l => l.Creator)
            .FirstOrDefaultAsync(l => l.Code == code);
}

using HomeLedger.API.Models;
using HomeLedger.API.Persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HomeLedger.API.Persistence.Repositories;

public class BillPaymentRepository(HomeLedgerDbContext context)
    : Repository<BillPayment>(context),
        IBillPaymentRepository
{
    public override async Task<IEnumerable<BillPayment>> GetAllAsync() =>
        await _dbSet.Include(bp => bp.Bill).Include(bp => bp.PaidBy).AsNoTracking().ToListAsync();

    public async Task<List<BillPayment>> GetAllByUserId(int userId) =>
        await _dbSet.Where(p => p.PaidById == userId).ToListAsync();
}

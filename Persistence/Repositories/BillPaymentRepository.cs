using HomeLedger.API.Models;
using HomeLedger.API.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HomeLedger.API.Persistence.Repositories;

public class BillPaymentRepository(HomeLedgerDbContext context) : Repository<BillPayment>(context)
{
    public override async Task<IEnumerable<BillPayment>> GetAllAsync() =>
        await _dbSet.Include(bp => bp.Bill).Include(bp => bp.PaidBy).AsNoTracking().ToListAsync();
}

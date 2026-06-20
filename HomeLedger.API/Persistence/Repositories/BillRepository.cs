using HomeLedger.API.Models;
using HomeLedger.API.Persistence;

namespace HomeLedger.API.Persistence.Repositories;

public class BillRepository(HomeLedgerDbContext context)
    : Repository<Bill>(context),
        IBillRepository { }

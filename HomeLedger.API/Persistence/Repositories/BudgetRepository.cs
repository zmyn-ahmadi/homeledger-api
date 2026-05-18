using HomeLedger.API.Models;

namespace HomeLedger.API.Persistence.Repositories;

public class BudgetRepository(HomeLedgerDbContext context)
    : Repository<Budget>(context),
        IBudgetRepository { }

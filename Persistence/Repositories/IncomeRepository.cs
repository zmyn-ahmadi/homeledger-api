using HomeLedger.API.Models;
using HomeLedger.API.Persistence;

public class IncomeRepository(HomeLedgerDbContext context) : Repository<Income>(context) { }

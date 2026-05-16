using HomeLedger.API.Models;
using HomeLedger.API.Persistence;

public class BillRepository(HomeLedgerDbContext context) : Repository<Bill>(context) { }

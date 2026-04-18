using HomeLedger.API.Models;
using HomeLedger.API.Persistence;

public class AccountRepository(HomeLedgerDbContext context) : Repository<Account>(context) { }

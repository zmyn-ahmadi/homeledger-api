using HomeLedger.API.Models;
using HomeLedger.API.Persistence;

public class TransactionRepository(HomeLedgerDbContext context)
    : Repository<Transaction>(context),
        ITransactionRepository { }

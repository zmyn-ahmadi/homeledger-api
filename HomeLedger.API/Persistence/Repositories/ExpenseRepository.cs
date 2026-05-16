using HomeLedger.API.Models;
using HomeLedger.API.Persistence;

public class ExpenseRepository(HomeLedgerDbContext context) : Repository<Expense>(context) { }

using HomeLedger.API.Persistence;
using HomeLedger.API.Persistence.Repositories;
using HomeLedger.API.Persistence.Repositories.Interfaces;

public class UnitOfWork : IUnitOfWork
{
    private readonly HomeLedgerDbContext _context;
    private readonly Dictionary<Type, object> _repositories = [];

    public UnitOfWork(HomeLedgerDbContext context)
    {
        _context = context;
        Categories = new CategoryRepository(_context);
        Users = new UserRepository(_context);
        BillPayments = new BillPaymentRepository(_context);
        Bills = new BillRepository(_context);
        Budgets = new BudgetRepository(_context);
        Expenses = new ExpenseRepository(_context);
        Incomes = new IncomeRepository(_context);
        Transactions = new TransactionRepository(_context);
        Accounts = new AccountRepository(_context);
    }

    public CategoryRepository Categories { get; }
    public UserRepository Users { get; }
    public BillPaymentRepository BillPayments { get; }
    public BillRepository Bills { get; }
    public BudgetRepository Budgets { get; }
    public ExpenseRepository Expenses { get; }
    public IncomeRepository Incomes { get; }
    public TransactionRepository Transactions { get; set; }
    public AccountRepository Accounts { get; set; }

    // Resolves a generic repo on-demand for any entity not needing a dedicated repo
    public IRepository<T> Repository<T>()
        where T : class
    {
        var type = typeof(T);
        if (!_repositories.TryGetValue(type, out var repo))
        {
            repo = new Repository<T>(_context);
            _repositories[type] = repo;
        }
        return (IRepository<T>)repo;
    }

    public Task<int> CommitAsync() => _context.SaveChangesAsync();

    public Task RollbackAsync() => _context.Database.RollbackTransactionAsync();

    public void Dispose() => _context.Dispose();
}

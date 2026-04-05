using HomeLedger.API.Persistence.Repositories;
using HomeLedger.API.Persistence.Repositories.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<T> Repository<T>()
        where T : class;
    CategoryRepository Categories { get; }
    UserRepository Users { get; }
    BillPaymentRepository BillPayments { get; }
    BillRepository Bills { get; }
    BudgetRepository Budgets { get; }
    ExpenseRepository Expenses { get; }
    IncomeRepository Incomes { get; }
    Task<int> CommitAsync();
    Task RollbackAsync();
}

using HomeLedger.API.Persistence.Repositories;
using HomeLedger.API.Persistence.Repositories.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<T> Repository<T>()
        where T : class;

    // Unit of work
    ICategoryRepository Categories { get; }
    IUserRepository Users { get; }
    IBillPaymentRepository BillPayments { get; }
    IBillRepository Bills { get; }
    IBudgetRepository Budgets { get; }
    IExpenseRepository Expenses { get; }
    IIncomeRepository Incomes { get; }
    IFamilyRepository Families { get; }

    // Database transactions functions
    Task<int> CommitAsync();
    Task RollbackAsync();
}

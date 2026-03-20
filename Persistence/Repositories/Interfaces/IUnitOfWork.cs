using HomeLedger.API.Persistence.Repositories;
using HomeLedger.API.Persistence.Repositories.Interfaces;

public interface IUnitOfWork : IDisposable
{
  IRepository<T> Repository<T>()
      where T : class;
  CategoryRepository Categories { get; }
  UserRepository Users { get; }
  Task<int> CommitAsync();
  Task RollbackAsync();
}

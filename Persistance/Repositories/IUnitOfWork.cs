namespace HomeLedger.API.Persistance.Repositories;

public interface IUnitOfWork : IAsyncDisposable
{
  IRepository<T> Repository<T>()
      where T : class;
  Task<int> SaveChangesAsync(CancellationToken ct = default);
}

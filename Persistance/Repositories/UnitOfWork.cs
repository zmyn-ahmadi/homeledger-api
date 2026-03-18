using HomeLedger.API.Data;

namespace HomeLedger.API.Persistance.Repositories;

public class UnitOfWork(HomeLedgerDbContext context) : IUnitOfWork
{
  private readonly HomeLedgerDbContext _context = context;
  private readonly Dictionary<Type, object> _repositories = [];

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

  public async Task<int> SaveChangesAsync(CancellationToken ct = default) =>
      await _context.SaveChangesAsync(ct);

  public async ValueTask DisposeAsync() => await _context.DisposeAsync();
}

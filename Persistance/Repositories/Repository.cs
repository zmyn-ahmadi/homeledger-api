using HomeLedger.API.Data;
using Microsoft.EntityFrameworkCore;

namespace HomeLedger.API.Persistance.Repositories;

public class Repository<T>(HomeLedgerDbContext context) : IRepository<T>
    where T : class
{
  protected readonly HomeLedgerDbContext _context = context;
  protected readonly DbSet<T> _dbSet = context.Set<T>();

  public async Task<IEnumerable<T>> GetAllAsync()
  {
    return await _dbSet.ToListAsync();
  }

  public async Task<T> GetByIdAsync(object id)
  {
    return await _dbSet.FindAsync(id);
  }

  public async void Insert(T obj)
  {
    await _dbSet.AddAsync(obj);
  }

  public void Update(T obj)
  {
    _dbSet.Attach(obj);
    _context.Entry(obj).State = EntityState.Modified;
  }

  public async void Delete(object id)
  {
    T existing = await _dbSet.FindAsync(id);
    _dbSet.Remove(existing);
  }

  public async void Save()
  {
    await _context.SaveChangesAsync();
  }
}

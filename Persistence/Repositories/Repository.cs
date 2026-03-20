using HomeLedger.API.Persistence;
using HomeLedger.API.Persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

public class Repository<T>(HomeLedgerDbContext context) : IRepository<T>
    where T : class
{
  protected readonly HomeLedgerDbContext _context = context;
  protected readonly DbSet<T> _dbSet = context.Set<T>();

  public IQueryable<T> GetQueryable() => _dbSet.AsQueryable();

  public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

  public async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);

  public async Task<T> AddAsync(T entity)
  {
    await _dbSet.AddAsync(entity);
    return entity;
  }

  public async Task<T> UpdateAsync(T entity)
  {
    _dbSet.Update(entity);
    return entity;
  }

  public async Task<bool> DeleteAsync(int id)
  {
    var entity = await GetByIdAsync(id);
    if (entity is null)
      return false;

    _dbSet.Remove(entity);
    return true;
  }

  public async Task<bool> ExistsAsync(int id) =>
      await _dbSet.AnyAsync(e => EF.Property<int>(e, "Id") == id);

  public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
}

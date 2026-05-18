using HomeLedger.API.Models;
using HomeLedger.API.Persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HomeLedger.API.Persistence.Repositories;

public class CategoryRepository(HomeLedgerDbContext context)
    : Repository<Category>(context),
        ICategoryRepository
{
    public async Task<IEnumerable<Category>> GetByUserIdAsync(int userId) =>
        await _dbSet.Where(c => c.OwnerId == userId).ToListAsync();

    public async Task<bool> NameExistsAsync(string name, int ownerId) =>
        await _dbSet.AnyAsync(c => c.Name == name && c.OwnerId == ownerId);
}

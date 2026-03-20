using HomeLedger.API.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeLedger.API.Persistence.Repositories;

public class UserRepository(HomeLedgerDbContext context) : Repository<User>(context)
{
  public async Task<User?> GetByEmailAsync(string email) =>
      await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
}

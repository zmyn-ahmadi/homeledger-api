using HomeLedger.API.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeLedger.API.Persistence.Repositories;

public class UserRepository(HomeLedgerDbContext context)
    : Repository<User>(context),
        IUserRepository
{
    public async Task<User?> GetByEmailAsync(string email) =>
        await _dbSet.FirstOrDefaultAsync(u => u.Email == email);

    public async Task<bool> UserExistsByIdAsync(int id) =>
        await _dbSet.AnyAsync(u => u.UserId == id);

    public void ToggleLazyLoading(bool status = false)
    {
        _context.ChangeTracker.LazyLoadingEnabled = status;
    }
}

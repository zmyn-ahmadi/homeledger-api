using HomeLedger.API.Models;

namespace HomeLedger.API.Persistence.Repositories.Interfaces;

public interface ICategoryRepository : IRepository<Category>
{
    Task<IEnumerable<Category>> GetByUserIdAsync(int userId);
    Task<bool> NameExistsAsync(string name, int ownerId);
}

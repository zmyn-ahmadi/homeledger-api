using HomeLedger.API.Models;
using HomeLedger.API.Persistence.Repositories.Interfaces;

public interface IBudgetRepository : IRepository<Budget>
{
    Task<List<Budget>> GetAllByUserIdAsync(int userId);
}

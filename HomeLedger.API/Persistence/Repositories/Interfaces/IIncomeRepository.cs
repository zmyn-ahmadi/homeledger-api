using HomeLedger.API.Models;
using HomeLedger.API.Persistence.Repositories.Interfaces;

public interface IIncomeRepository : IRepository<Income>
{
    Task<List<Income>> GetAllByUserIdAsync(int userId);
}

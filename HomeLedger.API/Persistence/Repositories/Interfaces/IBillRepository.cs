using HomeLedger.API.Models;
using HomeLedger.API.Persistence.Repositories.Interfaces;

public interface IBillRepository : IRepository<Bill>
{
    Task<List<Bill>> GetAllByUserIdAsync(int userId);
}

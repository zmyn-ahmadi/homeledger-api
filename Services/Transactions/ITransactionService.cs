using HomeLedger.API.Models;

namespace HomeLedger.API.Services.Transactions;

public interface ITransactionService
{
    Task<IEnumerable<Transaction>> GetAllAsync();
    Task<Transaction> GetByIdAsync(int id);
    Task<bool> DeleteAsync(int id);
}

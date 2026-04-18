using HomeLedger.API.Models;

public interface ITransactionService
{
    Task<IEnumerable<Transaction>> GetAllAsync();
    Task<Transaction> GetByIdAsync(int id);
    Task<Transaction> AddAsync(Transaction request);
    Task<bool> UpdateAsync(int id, Transaction request);
    Task<bool> DeleteAsync(int id);
}

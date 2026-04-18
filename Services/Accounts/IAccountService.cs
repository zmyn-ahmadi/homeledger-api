using HomeLedger.API.Models;

public interface IAccountService
{
    Task<IEnumerable<Account>> GetAllAsync();
    Task<Account> GetByIdAsync(int id);
    Task<Account> AddAsync(Account request);
    Task<bool> UpdateAsync(int id, Account request);
    Task<bool> DeleteAsync(int id);
}

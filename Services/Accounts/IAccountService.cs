using HomeLedger.API.Models;

public interface IAccountService
{
    Task<IEnumerable<AccountDTO>> GetAllAsync();
    Task<AccountDTO> GetByIdAsync(int id);
}

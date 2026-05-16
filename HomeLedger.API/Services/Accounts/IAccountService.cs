using HomeLedger.API.DTOs.AccountDTO;

public interface IAccountService
{
    Task<IEnumerable<AccountDTO>> GetAllAsync();
    Task<AccountDTO> GetByIdAsync(int id);
}

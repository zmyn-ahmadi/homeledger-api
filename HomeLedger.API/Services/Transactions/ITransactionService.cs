using HomeLedger.API.DTOs.TransactionDTO;

namespace HomeLedger.API.Services.Transactions;

public interface ITransactionService
{
    Task<IEnumerable<TransactionDTO>> GetAllByUserIdAsync();
    Task<TransactionDTO> GetByIdAsync(int id);
    Task<bool> DeleteAsync(int id);
}

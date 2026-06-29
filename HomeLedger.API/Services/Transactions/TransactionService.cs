public class TransactionService(
    IUnitOfWork _uow,
    ILogger<BillService> logger,
    IUserContext userContext
) : ITransactionService
{
    public async Task<IEnumerable<TransactionDTO>> GetAllByUserIdAsync() { }

    public async Task<TransactionDTO> GetByIdAsync(int id) { }

    public async Task<bool> DeleteAsync(int id) { }
}

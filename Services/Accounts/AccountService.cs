namespace HomeLedger.API.Services.Accounts;

public class AccountService(IUnitOfWork _uow) : IAccountService
{
    public Task<IEnumerable<AccountDTO>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<AccountDTO> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }
}

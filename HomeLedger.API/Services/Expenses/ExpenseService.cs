using HomeLedger.API.DTOs.ExpensesDTO;
using HomeLedger.API.Mappers;
using HomeLedger.API.Utils.UserContext;

public class ExpenseService(
    IUnitOfWork _uow,
    ExpenseMapper mapper,
    IUserContext userContext,
    ILogger<ExpenseService> logger
) : IExpenseService
{
    private readonly ExpenseMapper _mapper = mapper;
    private readonly ILogger<ExpenseService> _logger = logger;
    private readonly IUserContext _userContext = userContext;

    public async Task<ExpenseDTO> AddAsync(CreateExpenseDTO request)
    {
        // get the user's id from token
        var userId = _userContext.UserId;

        using var scope = _logger.BeginScope(
            new Dictionary<string, object> { ["OwnerId"] = userId }
        );

        var newExpense = _mapper.ToEntity(request);

        newExpense.OwnerId = userId;
        newExpense.CreatedAt = DateTime.UtcNow;
        newExpense.IsActive = true;

        await _uow.Expenses.AddAsync(newExpense);
        await _uow.CommitAsync();

        _logger.LogInformation(
            "Created Expense: {ExpenseId} for user {OwnerId}",
            newExpense.ExpenseId,
            newExpense.OwnerId
        );

        return _mapper.ToDto(newExpense);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var result = await _uow.Expenses.DeleteAsync(id);

        if (result)
        {
            await _uow.CommitAsync();
            _logger.LogInformation("Deleted Expense: {ExpenseId}", id);
        }
        else
        {
            _logger.LogWarning("Expense: {ExpenseId} not found for deletion", id);
        }

        return result;
    }

    public async Task<IEnumerable<ExpenseDTO>> GetAllAsync()
    {
        var expenses = await _uow.Expenses.GetAllAsync();
        return [.. expenses.Select(_mapper.ToDto)];
    }

    public async Task<ExpenseDTO?> GetByIdAsync(int id)
    {
        var expense = await _uow.Expenses.GetByIdAsync(id);
        if (expense is null)
        {
            _logger.LogWarning("Expense: {ExpenseId} not found.", id);
            return null;
        }
        return _mapper.ToDto(expense);
    }

    public async Task<bool> UpdateAsync(int id, UpdateExpenseDTO request)
    {
        var userId = _userContext.UserId;

        using var scope = _logger.BeginScope(
            new Dictionary<string, object> { ["OwnerId"] = userId, ["ExpenseId"] = id }
        );

        var existingExpense = await _uow.Expenses.GetByIdAsync(id);

        if (existingExpense is null)
        {
            _logger.LogWarning("{ExpenseId} not found for update", id);
            return false;
        }

        _mapper.UpdateEntity(request, existingExpense);
        existingExpense.UpdatedAt = DateTime.UtcNow;

        await _uow.Expenses.UpdateAsync(existingExpense);
        await _uow.CommitAsync();

        _logger.LogInformation("Updated Expense: {ExpenseId}", id);

        return true;
    }
}

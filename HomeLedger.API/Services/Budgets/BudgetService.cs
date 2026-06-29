using HomeLedger.API.DTOs.BudgetDTO;
using HomeLedger.API.Mappers;
using HomeLedger.API.Utils.UserContext;

public class BudgetService(
    IUnitOfWork _uow,
    BudgetMapper mapper,
    ILogger<BudgetService> logger,
    IUserContext userContext
) : IBudgetService
{
    private readonly BudgetMapper _mapper = mapper;
    private readonly ILogger<BudgetService> _logger = logger;
    private readonly IUserContext _ctx = userContext;

    public async Task<BudgetDTO> AddAsync(CreateBudgetDTO request)
    {
        var userId = _ctx.UserId;

        using var scope = _logger.BeginScope(
            new Dictionary<string, object> { ["OwnerId"] = userId }
        );

        var newBudget = _mapper.ToEntity(request);
        newBudget.OwnerId = userId;
        newBudget.CreatedAt = DateTime.UtcNow;
        newBudget.IsActive = true;

        await _uow.Budgets.AddAsync(newBudget);
        await _uow.CommitAsync();

        _logger.LogInformation(
            "Created Budget {BudgetId} for User {OwnerId}",
            newBudget.BudgetId,
            newBudget.OwnerId
        );

        return _mapper.ToDto(newBudget);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var result = await _uow.Budgets.DeleteAsync(id);
        if (result)
        {
            await _uow.CommitAsync();
            _logger.LogInformation("Deleted Budget {BudgetId}", id);
        }
        else
        {
            _logger.LogWarning("Budget: {BudgetId} not found for deletion.", id);
        }

        return result;
    }

    public async Task<IEnumerable<BudgetDTO>> GetAllAsync()
    {
        var budgets = await _uow.Budgets.GetAllAsync();
        var activeBudgets = budgets.Where(b => b.IsActive == true);
        return [.. activeBudgets.Select(_mapper.ToDto)];
    }

    public async Task<List<BudgetDTO>> GetAllByUserId()
    {
        var userId = _ctx.UserId;
        var budgets = await _uow.Budgets.GetAllByUserIdAsync(userId);
        var activeBudgets = budgets.Where(b => b.IsActive == true);
        return [.. activeBudgets.Select(_mapper.ToDto)];
    }

    public async Task<BudgetDTO?> GetByIdAsync(int id)
    {
        var budget = await _uow.Budgets.GetByIdAsync(id);

        if (budget is null)
        {
            _logger.LogWarning("Budget: {BudgetId} not found.", id);
            return null;
        }

        return _mapper.ToDto(budget);
    }

    public async Task<bool> UpdateAsync(int id, UpdateBudgetDTO request)
    {
        var userId = _ctx.UserId;

        using var scope = _logger.BeginScope(
            new Dictionary<string, object> { ["OwnerId"] = userId, ["BudgetId"] = id }
        );
        var existingBudget = await _uow.Budgets.GetByIdAsync(id);

        if (existingBudget is null)
        {
            _logger.LogWarning("BudgetId not found for update", id);
            return false;
        }

        _mapper.UpdateEntity(request, existingBudget);
        existingBudget.UpdatedAt = DateTime.UtcNow;

        await _uow.Budgets.UpdateAsync(existingBudget);
        await _uow.CommitAsync();

        _logger.LogInformation("Updated Budget {BudgetId}", id);
        return true;
    }
}

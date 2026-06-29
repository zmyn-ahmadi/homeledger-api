using HomeLedger.API.DTOs.IncomeDTO;
using HomeLedger.API.Mappers;
using HomeLedger.API.Services.Incomes;
using HomeLedger.API.Utils.UserContext;

public class IncomeService(
    IUnitOfWork _uow,
    IncomeMapper mapper,
    ILogger<IncomeService> logger,
    IUserContext userContext
) : IIncomeService
{
    private readonly IncomeMapper _mapper = mapper;
    private readonly ILogger<IncomeService> _logger = logger;
    private readonly IUserContext _userContext = userContext;

    public async Task<IncomeDTO> AddAsync(CreateIncomeDTO request)
    {
        var userId = _userContext.UserId;

        using var scope = _logger.BeginScope(
            new Dictionary<string, object> { ["OwnerId"] = userId }
        );

        var newIncome = _mapper.ToEntity(request);
        newIncome.OwnerId = userId;
        newIncome.CreatedAt = DateTime.UtcNow;
        newIncome.IsActive = true;

        await _uow.Incomes.AddAsync(newIncome);
        await _uow.CommitAsync();

        _logger.LogInformation(
            "Created Income {IncomeId} for user {OwnerId}",
            newIncome.IncomeId,
            newIncome.OwnerId
        );

        return _mapper.ToDto(newIncome);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var scope = _logger.BeginScope(new Dictionary<string, object> { ["IncomeId"] = id });

        var result = await _uow.Incomes.DeleteAsync(id);

        if (result)
        {
            await _uow.CommitAsync();
            _logger.LogInformation("Deleted Income {IncomeId}", id);
        }
        else
        {
            _logger.LogWarning("Income: {IncomeId} not found for deletion", id);
        }

        return result;
    }

    public async Task<IEnumerable<IncomeDTO>> GetAllAsync()
    {
        var incomes = await _uow.Incomes.GetAllAsync();
        return [.. incomes.Select(_mapper.ToDto)];
    }

    public async Task<IEnumerable<IncomeDTO>> GetAllByUserIdAsync()
    {
        var userId = _userContext.UserId;
        var incomes = await _uow.Incomes.GetAllByUserIdAsync(userId);
        return [.. incomes.Select(_mapper.ToDto)];
    }

    public async Task<IncomeDTO?> GetByIdAsync(int id)
    {
        var income = await _uow.Incomes.GetByIdAsync(id);
        if (income is null)
        {
            _logger.LogWarning("Income: {IncomeId} not found.", id);
            return null;
        }

        return _mapper.ToDto(income);
    }

    public async Task<bool> UpdateAsync(int id, UpdateIncomeDTO request)
    {
        var userId = _userContext.UserId;
        using var scope = _logger.BeginScope(
            new Dictionary<string, object> { ["OwnerId"] = userId, ["IncomeId"] = id }
        );

        var existingIncome = await _uow.Incomes.GetByIdAsync(id);
        if (existingIncome is null)
        {
            _logger.LogWarning("Income: {IncomeId} not found for update", id);
            return false;
        }

        _mapper.UpdateEntity(request, existingIncome);
        existingIncome.UpdatedAt = DateTime.UtcNow;

        await _uow.Incomes.UpdateAsync(existingIncome);
        await _uow.CommitAsync();

        _logger.LogInformation("Updated Income {IncomeId}", id);

        return true;
    }
}

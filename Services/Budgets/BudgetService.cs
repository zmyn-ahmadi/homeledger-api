using HomeLedger.API.DTOs.BudgetDTO;
using HomeLedger.API.Mappers;

public class BudgetService(IUnitOfWork uow, BudgetMapper mapper) : IBudgetService
{
    private readonly BudgetMapper _mapper = mapper;

    public async Task<BudgetDTO> AddAsync(CreateBudgetDTO request)
    {
        var newBudget = _mapper.ToEntity(request);
        newBudget.CreatedAt = DateTime.UtcNow;
        newBudget.IsActive = true;

        await uow.Budgets.AddAsync(newBudget);
        await uow.CommitAsync();

        return _mapper.ToDto(newBudget);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var result = await uow.Budgets.DeleteAsync(id);
        if (result)
            await uow.CommitAsync();

        return result;
    }

    public async Task<IEnumerable<BudgetDTO>> GetAllAsync()
    {
        var budgets = await uow.Budgets.GetAllAsync();
        return budgets.Select(_mapper.ToDto);
    }

    public async Task<BudgetDTO> GetByIdAsync(int id)
    {
        var budget = await uow.Budgets.GetByIdAsync(id);
        return _mapper.ToDto(budget);
    }

    public async Task<bool> UpdateAsync(int id, UpdateBudgetDTO request)
    {
        var existingBudget =
            await uow.Budgets.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Budget with ID: {id} not found.");

        _mapper.UpdateEntity(request, existingBudget);
        existingBudget.UpdatedAt = DateTime.UtcNow;

        await uow.Budgets.UpdateAsync(existingBudget);
        await uow.CommitAsync();

        return true;
    }
}

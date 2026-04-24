using HomeLedger.API.DTOs.IncomeDTO;
using HomeLedger.API.Mappers;
using HomeLedger.API.Services.Incomes;

public class IncomeService(IUnitOfWork uow, IncomeMapper mapper) : IIncomeService
{
    private readonly IncomeMapper _mapper = mapper;

    public async Task<IncomeDTO> AddAsync(CreateIncomeDTO request)
    {
        var newIncome = _mapper.ToEntity(request);
        newIncome.CreatedAt = DateTime.UtcNow;
        newIncome.IsActive = true;

        await uow.Incomes.AddAsync(newIncome);
        await uow.CommitAsync();

        return _mapper.ToDto(newIncome);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var result = await uow.Incomes.DeleteAsync(id);

        if (result)
            await uow.CommitAsync();

        return result;
    }

    public async Task<IEnumerable<IncomeDTO>> GetAllAsync()
    {
        var incomes = await uow.Incomes.GetAllAsync();
        return incomes.Select(_mapper.ToDto);
    }

    public async Task<IncomeDTO> GetByIdAsync(int id)
    {
        var income = await uow.Incomes.GetByIdAsync(id);
        return _mapper.ToDto(income);
    }

    public async Task<bool> UpdateAsync(int id, UpdateIncomeDTO request)
    {
        var existingIncome =
            await uow.Incomes.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Income with ID: {id} not found");

        _mapper.UpdateEntity(request, existingIncome);
        existingIncome.UpdatedAt = DateTime.UtcNow;

        await uow.Incomes.UpdateAsync(existingIncome);
        await uow.CommitAsync();

        return true;
    }
}

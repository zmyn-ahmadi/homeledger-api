using HomeLedger.API.DTOs.ExpensesDTO;
using HomeLedger.API.Mappers;

public class ExpenseService(IUnitOfWork uow, ExpenseMapper mapper) : IExpenseService
{
    private readonly ExpenseMapper _mapper = mapper;

    public async Task<ExpenseDTO> AddAsync(CreateExpenseDTO request)
    {
        var newExpense = _mapper.ToEntity(request);
        newExpense.CreatedAt = DateTime.UtcNow;
        newExpense.IsActive = true;

        await uow.Expenses.AddAsync(newExpense);
        await uow.CommitAsync();

        return _mapper.ToDto(newExpense);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var result = await uow.Expenses.DeleteAsync(id);

        if (result)
            await uow.CommitAsync();

        return result;
    }

    public async Task<IEnumerable<ExpenseDTO>> GetAllAsync()
    {
        var expenses = await uow.Expenses.GetAllAsync();
        return expenses.Select(_mapper.ToDto);
    }

    public async Task<ExpenseDTO> GetByIdAsync(int id)
    {
        var expense = await uow.Expenses.GetByIdAsync(id);
        return _mapper.ToDto(expense);
    }

    public async Task<bool> UpdateAsync(int id, UpdateExpenseDTO request)
    {
        var existingExpense =
            await uow.Expenses.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Expense with ID: {id} not found");

        _mapper.UpdateEntity(request, existingExpense);
        existingExpense.UpdatedAt = DateTime.UtcNow;

        await uow.Expenses.UpdateAsync(existingExpense);
        await uow.CommitAsync();

        return true;
    }
}

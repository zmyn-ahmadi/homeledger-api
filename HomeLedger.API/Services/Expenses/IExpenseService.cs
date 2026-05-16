using HomeLedger.API.DTOs.ExpensesDTO;

public interface IExpenseService
{
    Task<IEnumerable<ExpenseDTO>> GetAllAsync();
    Task<ExpenseDTO?> GetByIdAsync(int id);
    Task<ExpenseDTO> AddAsync(CreateExpenseDTO request);
    Task<bool> UpdateAsync(int id, UpdateExpenseDTO request);
    Task<bool> DeleteAsync(int id);
}

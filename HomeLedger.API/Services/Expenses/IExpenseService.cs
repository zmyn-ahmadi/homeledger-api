using HomeLedger.API.DTOs.ExpensesDTO;
using HomeLedger.API.Models;

public interface IExpenseService
{
    Task<IEnumerable<ExpenseDTO>> GetAllAsync();
    Task<ExpenseDTO?> GetByIdAsync(int id);
    Task<ExpenseDTO> AddAsync(CreateExpenseDTO request);
    Task<bool> UpdateAsync(int id, UpdateExpenseDTO request);
    Task<bool> DeleteAsync(int id);
    Task<List<ExpenseCategoryTotalDTO>> GetByCategoryAsync();
}

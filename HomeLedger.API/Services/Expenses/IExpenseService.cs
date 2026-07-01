using HomeLedger.API.DTOs.ExpensesDTO;
using HomeLedger.API.Models;

public interface IExpenseService
{
    Task<List<ExpenseDTO>> GetAllAsync();
    Task<ExpenseDTO?> GetByIdAsync(int id);
    Task<ExpenseDTO> AddAsync(CreateExpenseDTO request);
    Task<bool> UpdateAsync(int id, UpdateExpenseDTO request);
    Task<bool> DeleteAsync(int id);
    Task<List<ExpenseDTO>> GetAllByUserIdAsync();
    Task<List<ExpenseCategoryTotalDTO>> GetTotalByCategoryAsync();
}

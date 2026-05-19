using HomeLedger.API.DTOs.ExpensesDTO;
using HomeLedger.API.Models;
using HomeLedger.API.Persistence.Repositories.Interfaces;

public interface IExpenseRepository : IRepository<Expense>
{
    Task<List<ExpenseCategoryTotalDTO>> GetTotalByCategoryAsync(int userId);
}

using HomeLedger.API.DTOs.BillDTO;
using HomeLedger.API.DTOs.BudgetDTO;
using HomeLedger.API.DTOs.ExpensesDTO;
using HomeLedger.API.DTOs.FamilyDTO;
using HomeLedger.API.DTOs.IncomeDTO;
using HomeLedger.API.Models;

namespace HomeLedger.API.Services.Family;

public interface IFamilyService
{
    Task<IEnumerable<FamilyDTO>> GetAllAsync();
    Task<FamilyDTO?> GetByIdAsync(int id);
    Task<FamilyDTO> AddAsync(CreateFamilyDTO dto);
    Task<FamilyDTO?> UpdateAsync(int id, UpdateFamilyDTO dto);
    Task<bool> DeleteAsync(int id);
    Task<string> InviteAsync(string email);
    Task AcceptInvitationAsync(string code);
    Task<int?> GetCurrentUserFamilyIdAsync();

    // Bill
    Task<List<BillDTO?>> GetFamilyBillsAsync();
    Task<BillDTO?> GetFamilyBillAsync(int id);
    Task<BillDTO> AddFamilyBillAsync(CreateBillDTO req);

    // Budget
    Task<List<BudgetDTO?>> GetFamilyBudgetsAsync();
    Task<BudgetDTO?> GetFamilyBudgetAsync(int id);
    Task<BudgetDTO> AddFamilyBudgetAsync(CreateBudgetDTO req);

    // Expense
    Task<List<FamilyExpenseDTO>> GetFamilyExpensesAsync();

    // Income
    Task<List<FamilyIncomeDTO>> GetFamilyIncomesAsync();
}

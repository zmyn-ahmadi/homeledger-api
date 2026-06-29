using HomeLedger.API.DTOs.BudgetDTO;

public interface IBudgetService
{
    Task<IEnumerable<BudgetDTO>> GetAllAsync();
    Task<BudgetDTO?> GetByIdAsync(int id);
    Task<BudgetDTO> AddAsync(CreateBudgetDTO request);
    Task<bool> UpdateAsync(int id, UpdateBudgetDTO request);
    Task<bool> DeleteAsync(int id);
    Task<List<BudgetDTO>> GetAllByUserId();
}

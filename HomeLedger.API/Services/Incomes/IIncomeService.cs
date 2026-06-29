using HomeLedger.API.DTOs.IncomeDTO;

namespace HomeLedger.API.Services.Incomes;

public interface IIncomeService
{
    Task<IEnumerable<IncomeDTO>> GetAllAsync();
    Task<IEnumerable<IncomeDTO>> GetAllByUserIdAsync();
    Task<IncomeDTO?> GetByIdAsync(int id);
    Task<IncomeDTO> AddAsync(CreateIncomeDTO request);
    Task<bool> UpdateAsync(int id, UpdateIncomeDTO request);
    Task<bool> DeleteAsync(int id);
}

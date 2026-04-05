using HomeLedger.API.DTOs.IncomeDTO;

public interface IIncomeService
{
    Task<IEnumerable<IncomeDTO>> GetAllAsync();
    Task<IncomeDTO> GetByIdAsync(int id);
    Task<IncomeDTO> AddAsync(CreateIncomeDTO request);
    Task<bool> UpdateAsync(int id, UpdateIncomeDTO request);
    Task<bool> DeleteAsync(int id);
}

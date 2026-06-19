using HomeLedger.API.Models;

public interface IFamilyService
{
    Task<IEnumerable<Family>> GetAllAsync();
    Task<Family?> GetByIdAsync(int id);
    Task<Family> AddAsync(Family request);
    Task<Family> UpdateAsync(int id, Family request);
    Task<bool> DeleteAsync(int id);
}

using HomeLedger.API.Models;

namespace HomeLedger.API.Services.CategoryService;

public interface ICateogryService
{
    Task<IEnumerable<Category>> GetAllAsync();
    Task<Category?> GetByIdAsync(int id);
    Task<Category> AddAsync(Category category);
    Task<Category> UpdateAsync(int id, Category category);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}

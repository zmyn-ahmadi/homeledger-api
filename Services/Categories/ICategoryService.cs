using HomeLedger.API.DTOs.Category;

namespace HomeLedger.API.Services.Categories;

public interface ICategoryService
{
  Task<IEnumerable<CategoryDTO>> GetAllAsync();
  Task<CategoryDTO> GetByIdAsync(int id);
  Task<CategoryDTO> AddAsync(CreateCategoryDTO request);
  Task<CategoryDTO> UpdateAsync(int id, UpdateCategoryDTO request);
  Task<bool> DeleteAsync(int id);
  Task<bool> ExistsAsync(int id);
}

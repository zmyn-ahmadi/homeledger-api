using HomeLedger.API.Models;

namespace HomeLedger.API.Services.Categories;

public class CategoryService(IUnitOfWork uow) : ICategoryService
{
  public Task<IEnumerable<Category>> GetAllAsync()
  {
    return uow.Categories.GetAllAsync();
  }

  public Task<Category?> GetByIdAsync(int id)
  {
    return uow.Categories.GetByIdAsync(id);
  }

  public async Task<Category> AddAsync(Category category)
  {
    if (await uow.Categories.NameExistsAsync(category.Name))
      throw new InvalidOperationException($"Category '{category.Name}' already exists.");

    await uow.Categories.AddAsync(category);
    await uow.CommitAsync();
    return category;
  }

  public async Task<Category> UpdateAsync(int id, Category category)
  {
    var existing =
        await uow.Categories.GetByIdAsync(id)
        ?? throw new KeyNotFoundException($"Category with ID {id} not found.");

    existing.Name = category.Name;
    existing.Type = category.Type;
    existing.Icon = category.Icon;
    existing.Color = category.Color;

    await uow.Categories.UpdateAsync(existing);
    await uow.CommitAsync();
    return existing;
  }

  public async Task<bool> DeleteAsync(int id)
  {
    var result = await uow.Categories.DeleteAsync(id);
    if (result)
      await uow.CommitAsync();
    return result;
  }

  public Task<bool> ExistsAsync(int id)
  {
    return uow.Categories.ExistsAsync(id);
  }
}

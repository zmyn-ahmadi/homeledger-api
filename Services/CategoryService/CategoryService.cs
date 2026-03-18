using HomeLedger.API.Data;
using HomeLedger.API.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeLedger.API.Services.CategoryService;

public class CategoryService(HomeLedgerDbContext db) : ICateogryService
{
  private readonly HomeLedgerDbContext _db = db;

  public async Task<IEnumerable<Category>> GetAllAsync()
  {
    return await _db.Categories.ToListAsync();
  }

  public async Task<Category?> GetByIdAsync(int id)
  {
    return await _db.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
  }

  public async Task<Category> AddAsync(Category category)
  {
    // Check for duplicate name
    if (await _db.Categories.AnyAsync(c => c.Name == category.Name))
      throw new InvalidOperationException(
          $"Category with name '{category.Name}' already exists."
      );

    var newCategory = new Category
    {
      Name = category.Name,
      Type = category.Type,
      Icon = category.Icon,
      Color = category.Color,
    };

    await _db.Categories.AddAsync(newCategory);
    await _db.SaveChangesAsync();

    return newCategory;
  }

  public async Task<Category> UpdateAsync(int id, Category category)
  {
    var existingCat = await _db.Categories.FirstOrDefaultAsync(c => c.Id == id);
    if (existingCat == null)
      throw new KeyNotFoundException($"Category with ID {id} not found.");

    existingCat.Name = category.Name;
    existingCat.Type = category.Type;
    existingCat.Icon = category.Icon;
    existingCat.Color = category.Color;

    _db.Categories.Update(existingCat);
    await _db.SaveChangesAsync();

    return existingCat;
  }

  public async Task<bool> DeleteAsync(int id)
  {
    var category = await _db.Categories.FirstOrDefaultAsync(c => c.Id == id);
    if (category == null)
      return false;

    _db.Categories.Remove(category);
    await _db.SaveChangesAsync();
    return true;
  }

  public async Task<bool> ExistsAsync(int id)
  {
    var category = await _db.Categories.FirstOrDefaultAsync(c => c.Id == id);
    if (category == null)
      return false;
    return true;
  }
}

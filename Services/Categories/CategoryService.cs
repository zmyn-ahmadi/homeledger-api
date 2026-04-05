using HomeLedger.API.DTOs.CategoryDTO;
using HomeLedger.API.Mappers;

namespace HomeLedger.API.Services.Categories;

public class CategoryService(IUnitOfWork uow, CategoryMapper mapper) : ICategoryService
{
    private readonly CategoryMapper _mapper = mapper;

    public async Task<IEnumerable<CategoryDTO>> GetAllAsync()
    {
        var categories = await uow.Categories.GetAllAsync();
        return categories.Select(_mapper.ToDto);
    }

    public async Task<CategoryDTO> GetByIdAsync(int id)
    {
        var category = await uow.Categories.GetByIdAsync(id);
        return _mapper.ToDto(category);
    }

    public async Task<CategoryDTO> AddAsync(CreateCategoryDTO request)
    {
        var normalizedName = request.Name.Trim().ToLower();
        if (await uow.Categories.NameExistsAsync(normalizedName, request.OwnerId))
            throw new InvalidOperationException($"Category '{request.Name}' already exists.");

        var newCat = _mapper.ToEntity(request);
        newCat.CreatedAt = DateTime.UtcNow;
        newCat.IsActive = true;

        await uow.Categories.AddAsync(newCat);
        await uow.CommitAsync();

        return _mapper.ToDto(newCat);
    }

    public async Task<CategoryDTO> UpdateAsync(int id, UpdateCategoryDTO request)
    {
        var existing =
            await uow.Categories.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Category with ID {id} not found.");

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            var normalizedName = request.Name.Trim().ToLower();
            var categoryExists = await uow.Categories.NameExistsAsync(
                normalizedName,
                existing.OwnerId
            );
            if (categoryExists)
                throw new InvalidOperationException($"Category '{request.Name}' already exists.");
        }

        _mapper.UpdateEntity(request, existing);
        existing.UpdatedAt = DateTime.UtcNow;

        await uow.Categories.UpdateAsync(existing);
        await uow.CommitAsync();

        return _mapper.ToDto(existing);
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

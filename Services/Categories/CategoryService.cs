using AutoMapper;
using HomeLedger.API.DTOs.Category;
using HomeLedger.API.Models;

namespace HomeLedger.API.Services.Categories;

public class CategoryService(IUnitOfWork uow, IMapper mapper) : ICategoryService
{
    private readonly IMapper _mapper = mapper;

    public async Task<IEnumerable<CategoryDTO>> GetAllAsync()
    {
        var categories = await uow.Categories.GetAllAsync();
        return _mapper.Map<List<CategoryDTO>>(categories);
    }

    public async Task<CategoryDTO> GetByIdAsync(int id)
    {
        var category = await uow.Categories.GetByIdAsync(id);
        return _mapper.Map<CategoryDTO>(category);
    }

    public async Task<CategoryDTO> AddAsync(CreateCategoryDTO request)
    {
        var normalizedName = request.Name.Trim().ToLower();
        if (await uow.Categories.NameExistsAsync(normalizedName, request.OwnerId))
            throw new InvalidOperationException($"Category '{request.Name}' already exists.");

        var newCat = _mapper.Map<Category>(request);
        newCat.CreatedAt = DateTime.UtcNow;

        await uow.Categories.AddAsync(newCat);
        await uow.CommitAsync();

        return _mapper.Map<CategoryDTO>(newCat);
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

        _mapper.Map(request, existing);
        existing.UpdatedAt = DateTime.UtcNow;

        await uow.Categories.UpdateAsync(existing);
        await uow.CommitAsync();

        return _mapper.Map<CategoryDTO>(existing);
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

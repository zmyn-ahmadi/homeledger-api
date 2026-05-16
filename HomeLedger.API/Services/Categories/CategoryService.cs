using HomeLedger.API.DTOs.CategoryDTO;
using HomeLedger.API.Mappers;

namespace HomeLedger.API.Services.Categories;

public class CategoryService(
    IUnitOfWork _uow,
    CategoryMapper mapper,
    ILogger<CategoryService> logger
) : ICategoryService
{
    private readonly CategoryMapper _mapper = mapper;
    private readonly ILogger<CategoryService> _logger = logger;

    public async Task<IEnumerable<CategoryDTO>> GetAllAsync()
    {
        var categories = await _uow.Categories.GetAllAsync();
        return [.. categories.Select(_mapper.ToDto)];
    }

    public async Task<CategoryDTO?> GetByIdAsync(int id)
    {
        var category = await _uow.Categories.GetByIdAsync(id);
        if (category is null)
        {
            _logger.LogWarning("Category: {CategoryId} not found.", id);
            return null;
        }
        return _mapper.ToDto(category);
    }

    public async Task<CategoryDTO> AddAsync(CreateCategoryDTO request)
    {
        using var scope = _logger.BeginScope(
            new Dictionary<string, object> { ["OwnerId"] = request.OwnerId }
        );

        var normalizedName = request.Name.Trim().ToLower();
        if (await _uow.Categories.NameExistsAsync(normalizedName, request.OwnerId))
        {
            _logger.LogWarning("Category {Name} already exists", request.Name);
            throw new InvalidOperationException($"Category '{request.Name}' already exists.");
        }

        var newCat = _mapper.ToEntity(request);
        newCat.CreatedAt = DateTime.UtcNow;
        newCat.IsActive = true;

        await _uow.Categories.AddAsync(newCat);
        await _uow.CommitAsync();

        _logger.LogInformation(
            "Created Category: {CategoryId} for user {OwnerId}",
            newCat.CategoryId,
            newCat.OwnerId
        );

        return _mapper.ToDto(newCat);
    }

    public async Task<bool> UpdateAsync(int id, UpdateCategoryDTO request)
    {
        using var scope = _logger.BeginScope(
            new Dictionary<string, object> { ["CategoryId"] = id }
        );

        var existing = await _uow.Categories.GetByIdAsync(id);
        if (existing is null)
        {
            _logger.LogWarning("{CategoryId} not found for update", id);
            return false;
        }

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            var normalizedName = request.Name.Trim().ToLower();
            var categoryExists = await _uow.Categories.NameExistsAsync(
                normalizedName,
                existing.OwnerId
            );
            if (categoryExists)
            {
                _logger.LogWarning("{CategoryName} already exists", request.Name);
                return false;
            }
        }

        _mapper.UpdateEntity(request, existing);
        existing.UpdatedAt = DateTime.UtcNow;

        await _uow.Categories.UpdateAsync(existing);
        await _uow.CommitAsync();

        _logger.LogInformation("Updated category {CategoryId}", id);

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var scope = _logger.BeginScope(
            new Dictionary<string, object> { ["CategoryId"] = id }
        );

        var result = await _uow.Categories.DeleteAsync(id);
        if (result)
        {
            await _uow.CommitAsync();
            _logger.LogInformation("Deleted Category {CategoryId}", id);
        }
        else
        {
            _logger.LogWarning("Category: {CategoryId} not found for deletion", id);
        }

        return result;
    }

    public Task<bool> ExistsAsync(int id)
    {
        return _uow.Categories.ExistsAsync(id);
    }
}

using HomeLedger.API.DTOs.CategoryDTO;
using HomeLedger.API.Models;
using Riok.Mapperly.Abstractions;

namespace HomeLedger.API.Mappers;

[Mapper]
public partial class CategoryMapper
{
    // 1. Standard Generated mapping
    // [MapperIgnoreSource(nameof(Category.OwnerId))]
    // [MapperIgnoreSource(nameof(Category.Owner))]
    // [MapperIgnoreSource(nameof(Category.Bills))]
    // [MapperIgnoreSource(nameof(Category.Budgets))]
    // [MapperIgnoreSource(nameof(Category.IsActive))]
    // [MapperIgnoreSource(nameof(Category.Expenses))]
    // [MapperIgnoreSource(nameof(Category.CreatedAt))]
    // [MapperIgnoreSource(nameof(Category.UpdatedAt))]
    // [MapperIgnoreSource(nameof(Category.Incomes))]
    public partial CategoryDTO ToDto(Category category);

    // 2. Update Mapping
    public partial void UpdateEntity(UpdateCategoryDTO dto, Category entity);

    // 3. create mapping with custom logic
    public Category ToEntity(CreateCategoryDTO dto)
    {
        // call the generated partial method do do the heavy lifting
        var entity = ToEntityInternal(dto);
        entity.Name = dto.Name.Trim().ToLower();

        return entity;
    }

    // this is the method mapperly will generate
    [MapProperty(nameof(CreateCategoryDTO.Name), nameof(Category.Name))]
    private partial Category ToEntityInternal(CreateCategoryDTO dto);
}

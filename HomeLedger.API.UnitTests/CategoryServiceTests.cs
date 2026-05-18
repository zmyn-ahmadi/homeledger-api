using HomeLedger.API.DTOs.CategoryDTO;
using HomeLedger.API.Mappers;
using HomeLedger.API.Models;
using HomeLedger.API.Persistence.Repositories.Interfaces;
using HomeLedger.API.Services.Categories;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Xunit;

namespace HomeLedger.API.UnitTests;

public class CategoryServiceTests
{
    private readonly IUnitOfWork _uow = Substitute.For<IUnitOfWork>();
    private readonly ICategoryRepository _categories = Substitute.For<ICategoryRepository>();
    private readonly CategoryMapper _mapper = new();
    private readonly ILogger<CategoryService> _logger = Substitute.For<ILogger<CategoryService>>();
    private readonly CategoryService _sut;

    public CategoryServiceTests()
    {
        _uow.Categories.Returns(_categories);
        _sut = new CategoryService(_uow, _mapper, _logger);
    }

    [Fact]
    public async Task AddAsync_throws_when_name_already_exists()
    {
        var request = new CreateCategoryDTO { Name = "Groceries", OwnerId = 1 };
        _categories.NameExistsAsync("groceries", 1).Returns(true);

        var ex = await Should.ThrowAsync<InvalidOperationException>(() => _sut.AddAsync(request));

        ex.Message.ShouldContain("already exists");

        await _categories.DidNotReceive().AddAsync(Arg.Any<Category>());
        await _uow.DidNotReceive().CommitAsync();
    }
}

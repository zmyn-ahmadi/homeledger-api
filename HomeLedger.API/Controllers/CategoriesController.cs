using HomeLedger.API.DTOs.CategoryDTO;
using HomeLedger.API.Services.Categories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeLedger.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
public class CategoriesController(ICategoryService cateogryService) : ControllerBase
{
    private readonly ICategoryService _service = cateogryService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategories()
    {
        var categories = await _service.GetAllAsync();
        var categoriesList = categories as IList<CategoryDTO> ?? [.. categories];

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return categoriesList.Count == 0 ? NoContent() : Ok(categoriesList);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoryDTO?>> GetCategory(int id)
    {
        var category = await _service.GetByIdAsync(id);

        if (category == null)
            return NotFound(new { Message = $"Category with ID {id} not found." });

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(category);
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDTO>> PostCategory(
        [FromBody] CreateCategoryDTO createCategory
    )
    {
        if (createCategory == null)
            return BadRequest("Request body is missing or invalid");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var newCategory = await _service.AddAsync(createCategory);

        return CreatedAtAction(
            nameof(GetCategory),
            new { id = newCategory.CategoryId },
            newCategory
        );
    }

    [HttpPut("id:int")]
    public async Task<ActionResult<CategoryDTO>> PutCategory(
        [FromRoute] int id,
        [FromBody] UpdateCategoryDTO updateCategory
    )
    {
        if (updateCategory == null)
            return BadRequest("Request body is missing or invalid");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var updatedCategory = await _service.UpdateAsync(id, updateCategory);
        return updatedCategory ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<bool>> DeleteCategory([FromRoute] int id)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!await _service.DeleteAsync(id))
            return NotFound($"Category with ID {id} not found.");

        return NoContent();
    }
}

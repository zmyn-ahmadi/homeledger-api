using HomeLedger.API.DTOs.BudgetDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeLedger.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BudgetController(IBudgetService budgetService) : ControllerBase
{
    private readonly IBudgetService _service = budgetService;

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<BudgetDTO>> GetBudgets()
    {
        var budgets = await _service.GetAllAsync();
        if (!budgets.Any())
            return NoContent();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(budgets);
    }

    [Authorize]
    [HttpGet("id:int")]
    public async Task<ActionResult<BudgetDTO>> GetBudget(int id)
    {
        var budget = await _service.GetByIdAsync(id);
        if (budget == null)
            return NotFound(new { Message = $"Budget with ID {id} not found." });

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(budget);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<BudgetDTO>> PostBudget([FromBody] CreateBudgetDTO createBudget)
    {
        if (createBudget == null)
            return BadRequest("Request body is missing or invalid");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var newBudget = await _service.AddAsync(createBudget);
        return CreatedAtAction(nameof(GetBudget), new { id = newBudget.BudgetId }, newBudget);
    }

    [Authorize]
    [HttpPut("id:int")]
    public async Task<ActionResult<BudgetDTO>> PutBudget(
        [FromRoute] int id,
        [FromBody] UpdateBudgetDTO updateBudget
    )
    {
        if (updateBudget == null)
            return BadRequest("Request body is missing or invalid");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var updatedBudget = await _service.UpdateAsync(id, updateBudget);
        return Ok(updatedBudget);
    }

    [Authorize]
    [HttpDelete("id:int")]
    public async Task<ActionResult<bool>> DeleteBudget([FromRoute] int id)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!await _service.DeleteAsync(id))
            return NotFound($"Budget with ID:{id} not found.");
        return NoContent();
    }
}

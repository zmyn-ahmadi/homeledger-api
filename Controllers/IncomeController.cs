using HomeLedger.API.DTOs.IncomeDTO;
using HomeLedger.API.Services.Incomes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class IncomeController(IIncomeService incomeService) : ControllerBase
{
    private readonly IIncomeService _service = incomeService;

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IncomeDTO>> GetIncomes()
    {
        var incomes = await _service.GetAllAsync();
        if (!incomes.Any())
            return NoContent();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(incomes);
    }

    [Authorize]
    [HttpGet("id:int")]
    public async Task<ActionResult<IncomeDTO>> GetIncome(int id)
    {
        var income = await _service.GetByIdAsync(id);
        if (income == null)
            return NotFound(new { Message = $"Income with ID {id} not found." });

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(income);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<IncomeDTO>> PostIncome([FromBody] CreateIncomeDTO createIncome)
    {
        if (createIncome == null)
            return BadRequest("Request body is missing or invalid");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var newIncome = await _service.AddAsync(createIncome);
        return CreatedAtAction(nameof(GetIncome), new { id = newIncome.IncomeId }, newIncome);
    }

    [Authorize]
    [HttpPut("id:int")]
    public async Task<ActionResult<IncomeDTO>> PutIncome(
        [FromRoute] int id,
        [FromBody] UpdateIncomeDTO updateIncome
    )
    {
        if (updateIncome == null)
            return BadRequest("Request body is missing or invalid");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var updatedIncome = await _service.UpdateAsync(id, updateIncome);
        return Ok(updatedIncome);
    }

    [Authorize]
    [HttpDelete("id:int")]
    public async Task<ActionResult<bool>> DeleteIncome([FromRoute] int id)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!await _service.DeleteAsync(id))
            return NotFound($"Income with ID:{id} not found.");
        return NoContent();
    }
}

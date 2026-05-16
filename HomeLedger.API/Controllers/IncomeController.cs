using HomeLedger.API.DTOs.IncomeDTO;
using HomeLedger.API.Services.Incomes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Authorize]
[Route("api/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
public class IncomeController(IIncomeService incomeService) : ControllerBase
{
    private readonly IIncomeService _service = incomeService;

    [HttpGet]
    public async Task<ActionResult<IncomeDTO>> GetIncomes()
    {
        var incomes = await _service.GetAllAsync();
        var incomesList = incomes as IList<IncomeDTO> ?? [.. incomes];

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return incomesList.Count == 0 ? NoContent() : Ok(incomesList);
    }

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
        return updatedIncome ? NoContent() : NotFound();
    }

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

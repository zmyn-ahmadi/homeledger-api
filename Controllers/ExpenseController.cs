using HomeLedger.API.DTOs.ExpensesDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Authorize]
[Route("api/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
public class ExpenseController(IExpenseService expenseService) : ControllerBase
{
    private readonly IExpenseService _service = expenseService;

    [HttpGet]
    public async Task<ActionResult<ExpenseDTO>> GetExpenses()
    {
        var expenses = await _service.GetAllAsync();
        var expensesList = expenses as IList<ExpenseDTO> ?? [.. expenses];

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return expensesList.Count == 0 ? NoContent() : Ok(expensesList);
    }

    [HttpGet("id:int")]
    public async Task<ActionResult<ExpenseDTO?>> GetExpense(int id)
    {
        var expense = await _service.GetByIdAsync(id);
        if (expense == null)
            return NotFound(new { Message = $"Expense with ID {id} not found." });

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(expense);
    }

    [HttpPost]
    public async Task<ActionResult<ExpenseDTO>> PostExpense(
        [FromBody] CreateExpenseDTO createExpense
    )
    {
        if (createExpense == null)
            return BadRequest("Request body is missing or invalid");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var newExpense = await _service.AddAsync(createExpense);
        return CreatedAtAction(nameof(GetExpense), new { id = newExpense.ExpenseId }, newExpense);
    }

    [HttpPut("id:int")]
    public async Task<ActionResult<ExpenseDTO>> PutExpense(
        [FromRoute] int id,
        [FromBody] UpdateExpenseDTO updateExpense
    )
    {
        if (updateExpense == null)
            return BadRequest("Request body is missing or invalid");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var updatedExpense = await _service.UpdateAsync(id, updateExpense);
        return updatedExpense ? NoContent() : NotFound();
    }

    [Authorize]
    [HttpDelete("id:int")]
    public async Task<ActionResult<bool>> DeleteExpense([FromRoute] int id)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!await _service.DeleteAsync(id))
            return NotFound($"Expense with ID:{id} not found.");
        return NoContent();
    }
}

using HomeLedger.API.DTOs.ExpensesDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ExpenseController(IExpenseService expenseService) : ControllerBase
{
    private readonly IExpenseService _service = expenseService;

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<ExpenseDTO>> GetExpenses()
    {
        var expenses = await _service.GetAllAsync();
        if (!expenses.Any())
            return NoContent();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(expenses);
    }

    [Authorize]
    [HttpGet("id:int")]
    public async Task<ActionResult<ExpenseDTO>> GetExpense(int id)
    {
        var expense = await _service.GetByIdAsync(id);
        if (expense == null)
            return NotFound(new { Message = $"Expense with ID {id} not found." });

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(expense);
    }

    [Authorize]
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

    [Authorize]
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
        return Ok(updatedExpense);
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

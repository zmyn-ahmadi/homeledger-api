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

    [Authorize(Policy = "SuperUserOnly")]
    [HttpGet("all")]
    public async Task<ActionResult<ExpenseDTO>> GetExpenses()
    {
        var expenses = await _service.GetAllAsync();
        var expensesList = expenses as IList<ExpenseDTO> ?? [.. expenses];
        return expensesList.Count == 0 ? NoContent() : Ok(expensesList);
    }

    [HttpGet]
    public async Task<ActionResult<ExpenseDTO>> GetExpensesByUserId()
    {
        var expenses = await _service.GetAllByUserIdAsync();
        var expensesList = expenses as IList<ExpenseDTO> ?? [.. expenses];
        return expensesList.Count == 0 ? NoContent() : Ok(expensesList);
    }

    [HttpGet("category-totals")]
    public async Task<ActionResult<IEnumerable<ExpenseCategoryTotalDTO>>> GetCategoryTotals()
    {
        var totals = await _service.GetTotalByCategoryAsync();
        return totals.Count == 0 ? NoContent() : Ok(totals);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ExpenseDTO>> GetExpense(int id)
    {
        var expense = await _service.GetByIdAsync(id);
        if (expense == null)
            return NotFound(new { Message = $"Expense with ID {id} not found." });
        return Ok(expense);
    }

    [HttpPost]
    public async Task<ActionResult<ExpenseDTO>> PostExpense(
        [FromBody] CreateExpenseDTO createExpense
    )
    {
        if (createExpense == null)
            return BadRequest("Request body is missing or invalid");
        var newExpense = await _service.AddAsync(createExpense);
        return CreatedAtAction(nameof(GetExpense), new { id = newExpense.ExpenseId }, newExpense);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutExpense(
        [FromRoute] int id,
        [FromBody] UpdateExpenseDTO updateExpense
    )
    {
        if (updateExpense == null)
            return BadRequest("Request body is missing or invalid");

        var updatedExpense = await _service.UpdateAsync(id, updateExpense);
        return updatedExpense ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteExpense([FromRoute] int id)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!await _service.DeleteAsync(id))
            return NotFound($"Expense with ID:{id} not found.");
        return NoContent();
    }
}

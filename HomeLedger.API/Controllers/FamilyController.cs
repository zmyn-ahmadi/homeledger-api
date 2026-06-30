using HomeLedger.API.DTOs.BillDTO;
using HomeLedger.API.DTOs.BudgetDTO;
using HomeLedger.API.DTOs.ExpensesDTO;
using HomeLedger.API.DTOs.FamilyDTO;
using HomeLedger.API.DTOs.IncomeDTO;
using HomeLedger.API.Models;
using HomeLedger.API.Services.Family;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeLedger.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
public class FamilyController(IFamilyService familyService) : ControllerBase
{
    private readonly IFamilyService _service = familyService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FamilyDTO>>> GetFamilies()
    {
        var fams = await _service.GetAllAsync();
        var famsList = fams as IList<FamilyDTO> ?? [.. fams];

        return famsList.Count == 0 ? NoContent() : Ok(famsList);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<FamilyDTO?>> GetFamily(int id)
    {
        var fam = await _service.GetByIdAsync(id);
        return fam != null
            ? Ok(fam)
            : NotFound(new { Message = $"Family with ID {id} not found." });
    }

    [HttpPost]
    public async Task<ActionResult<FamilyDTO>> PostFamily([FromBody] CreateFamilyDTO createFam)
    {
        if (createFam == null)
            return BadRequest("Request body is missing or invalid");

        var newFam = await _service.AddAsync(createFam);
        return CreatedAtAction(nameof(GetFamily), new { id = newFam.FamilyId }, newFam);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<FamilyDTO>> PutFamily(
        [FromRoute] int id,
        [FromBody] UpdateFamilyDTO updateFam
    )
    {
        if (updateFam == null)
            return BadRequest("Request body is missing or invalid");

        var updatedFam = await _service.UpdateAsync(id, updateFam);
        return updatedFam == null
            ? NotFound(new { Message = $"Family with ID {id} not found." })
            : Ok(updatedFam);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteFamily(int id)
    {
        var result = await _service.DeleteAsync(id);
        return result ? NoContent() : NotFound(new { Message = $"Family with ID {id} not found" });
    }

    [HttpPost("invitations")]
    public async Task<ActionResult<string>> CreateInvitation()
    {
        var link = await _service.InviteAsync();
        return Ok(link);
    }

    [HttpPost("invitations/{code}/accept")]
    public async Task<IActionResult> AcceptInvitation(string code)
    {
        await _service.AcceptInvitationAsync(code);
        return NoContent();
    }

    [HttpGet("bills")]
    public async Task<ActionResult<IEnumerable<BillDTO>>> GetFamilyBills()
    {
        var bills = await _service.GetFamilyBillsAsync();
        return bills.Count == 0 ? NoContent() : Ok(bills);
    }

    [HttpGet("bill/{id:int}")]
    public async Task<ActionResult<BillDTO>> GetFamilyBill(int id)
    {
        var bill = await _service.GetFamilyBillAsync(id);
        return bill != null ? Ok(bill) : NotFound(new { Message = $"Bill {id} not found." });
    }

    [HttpPost("bill")]
    public async Task<ActionResult<BillDTO>> PostFamilyBill(CreateBillDTO req)
    {
        if (req is null)
            return BadRequest("Request body is missing or invalid");

        var newBill = await _service.AddFamilyBillAsync(req);
        return CreatedAtAction(nameof(GetFamilyBill), new { id = newBill.BillId }, newBill);
    }

    [HttpGet("budgets")]
    public async Task<ActionResult<IEnumerable<BudgetDTO>>> GetFamilyBudgets()
    {
        var budgets = await _service.GetFamilyBudgetsAsync();
        return budgets.Count == 0 ? NoContent() : Ok(budgets);
    }

    [HttpGet("budgets/{id:int}")]
    public async Task<ActionResult<BudgetDTO>> GetFamilyBudget(int id)
    {
        var budget = await _service.GetFamilyBudgetAsync(id);
        return budget != null ? Ok(budget) : NotFound(new { Message = $"Budget {id} not found." });
    }

    [HttpPost("budget")]
    public async Task<ActionResult<BudgetDTO>> PostFamilyBudget(CreateBudgetDTO req)
    {
        if (req is null)
            return BadRequest("Request body is missing or invalid");

        var newBudget = await _service.AddFamilyBudgetAsync(req);
        return CreatedAtAction(nameof(GetFamilyBudget), new { id = newBudget.BudgetId }, newBudget);
    }

    [HttpGet("expenses")]
    public async Task<ActionResult<IEnumerable<ExpenseDTO>>> GetFamilyExpenses()
    {
        var expenses = await _service.GetFamilyExpensesAsync();
        return expenses.Count == 0 ? NoContent() : Ok(expenses);
    }

    [HttpGet("incomes")]
    public async Task<ActionResult<IEnumerable<IncomeDTO>>> GetFamilyIncomes()
    {
        var incomes = await _service.GetFamilyIncomesAsync();
        return incomes.Count == 0 ? NoContent() : Ok(incomes);
    }
}

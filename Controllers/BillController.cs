using HomeLedger.API.DTOs.BillDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeLedger.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
public class BillController(IBillService billService) : ControllerBase
{
    private readonly IBillService _service = billService;

    [HttpGet]
    public async Task<ActionResult<BillDTO>> GetBills()
    {
        var bills = await _service.GetAllAsync();
        var billsList = bills as IList<BillDTO> ?? [.. bills];

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return billsList.Count == 0 ? NoContent() : Ok(billsList);
    }

    [HttpGet("id:int")]
    public async Task<ActionResult<BillDTO>> GetBill(int id)
    {
        var bill = await _service.GetByIdAsync(id);
        if (bill == null)
            return NotFound(new { Message = $"Bill with ID {id} not found." });

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(bill);
    }

    [HttpPost]
    public async Task<ActionResult<BillDTO>> PostBill([FromBody] CreateBillDTO createBill)
    {
        if (createBill == null)
            return BadRequest("Request body is missing or invalid");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var newBill = await _service.AddAsync(createBill);
        return CreatedAtAction(nameof(GetBill), new { id = newBill.BillId }, newBill);
    }

    [HttpPut("id:int")]
    public async Task<ActionResult<BillDTO>> PutBill(
        [FromRoute] int id,
        [FromBody] UpdateBillDTO updateBill
    )
    {
        if (updateBill == null)
            return BadRequest("Request body is missing or invalid");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var updatedBill = await _service.UpdateAsync(id, updateBill);
        return updatedBill ? NoContent() : NotFound();
    }

    [HttpDelete("id:int")]
    public async Task<ActionResult<bool>> DeleteBill([FromRoute] int id)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!await _service.DeleteAsync(id))
            return NotFound($"Bill with ID:{id} not found.");
        return NoContent();
    }
}

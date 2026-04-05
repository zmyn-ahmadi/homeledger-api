using HomeLedger.API.DTOs.BillPaymentsDTO;
using HomeLedger.API.Models;
using HomeLedger.API.Services.BillPayments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeLedger.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BillPaymentController(IBillPaymentService billPaymentService) : ControllerBase
{
    private readonly IBillPaymentService _service = billPaymentService;

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BillPayment>>> GetBillPayments()
    {
        var payments = await _service.GetAllAsync();
        if (!payments.Any())
            return NoContent();

        return Ok(payments);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<BillPayment>> CreateBillPayment(CreateBillPaymentDTO request)
    {
        await _service.AddAsync(request);
        return CreatedAtAction(nameof(GetBillPayment), new { id = request.BillPaymentId }, request);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<BillPayment>> GetBillPayment(int id)
    {
        try
        {
            var payment = await _service.GetByIdAsync(id);
            if (payment == null)
                return NotFound();

            return Ok(payment);
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine("Error retrieving BillPayment with id {id}.", id);
            return StatusCode(500, "Internal server error.");
        }
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBillPayment(int id, UpdateBillPaymentDTO billPayment)
    {
        if (id != billPayment.BillId)
            return BadRequest();

        await _service.UpdateAsync(id, billPayment);
        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBillPayment(int id)
    {
        var payment = await _service.GetByIdAsync(id);
        if (payment == null)
            return NotFound();

        await _service.DeleteAsync(id);
        return NoContent();
    }
}

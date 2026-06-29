using HomeLedger.API.DTOs.BillPaymentsDTO;
using HomeLedger.API.Models;
using HomeLedger.API.Services.BillPayments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeLedger.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
public class BillPaymentController(IBillPaymentService billPaymentService) : ControllerBase
{
    private readonly IBillPaymentService _service = billPaymentService;

    // GET: api/billpayments
    [Authorize(Policy = "SuperUserOnly")]
    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<BillPaymentDTO>>> GetBillPayments()
    {
        var payments = await _service.GetAllAsync();
        var paymentsList = payments as IList<BillPaymentDTO> ?? [.. payments];
        return paymentsList.Count == 0 ? NoContent() : Ok(paymentsList);
    }

    [HttpGet]
    public async Task<ActionResult<BillPaymentDTO>> GetPaymentsByUserId()
    {
        var payments = await _service.GetAllByUserIdAsync();
        var paymentsList = payments as IList<BillPaymentDTO> ?? [.. payments];
        return paymentsList.Count == 0 ? NoContent() : Ok(paymentsList);
    }

    [HttpPost]
    public async Task<ActionResult<BillPaymentDTO>> CreateBillPayment(CreateBillPaymentDTO request)
    {
        var created = await _service.AddAsync(request);
        return CreatedAtAction(nameof(GetBillPayment), new { id = created.BillPaymentId }, created);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BillPayment?>> GetBillPayment(int id)
    {
        var payment = await _service.GetByIdAsync(id);
        return payment is null ? NotFound() : Ok(payment);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateBillPayment(int id, UpdateBillPaymentDTO billPayment)
    {
        // if (id != billPayment.BillPaymentId)
        //     return BadRequest();

        var updated = await _service.UpdateAsync(id, billPayment);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteBillPayment(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}

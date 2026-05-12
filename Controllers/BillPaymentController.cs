using System.Diagnostics;
using HomeLedger.API.DTOs.BillPaymentsDTO;
using HomeLedger.API.Models;
using HomeLedger.API.Services.BillPayments;
using HomeLedger.API.Services.Correlation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeLedger.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
public class BillPaymentController : ControllerBase
{
    private readonly IBillPaymentService _service;
    private readonly ICorrelationIdAccessor _correlationIdAccessor;
    private readonly ILogger<BillPaymentController> _logger;

    public BillPaymentController(
        IBillPaymentService billPaymentService,
        ILogger<BillPaymentController> logger,
        ICorrelationIdAccessor correlationIdAccessor
    )
    {
        _logger = logger;
        _service = billPaymentService;
        _correlationIdAccessor = correlationIdAccessor;
        _logger.LogInformation("BillPaymentController instantiated.");
    }

    // GET: api/billpayments
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BillPaymentDTO>>> GetBillPayments()
    {
        var correlationId = _correlationIdAccessor.GetCorrelationId();
        using var scope = _logger.BeginScope(
            new Dictionary<string, object> { ["CorrelationId"] = correlationId }
        );

        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation("HTTP GET /api/BillPayments called");

        var payments = await _service.GetAllAsync();
        stopwatch.Stop();

        var paymentsList = payments as IList<BillPaymentDTO> ?? payments.ToList();

        if (paymentsList.Count == 0)
        {
            _logger.LogWarning(
                "No BillPayments found. ElapsedMs={ElapsedMs}",
                stopwatch.ElapsedMilliseconds
            );
            return NoContent();
        }

        _logger.LogInformation(
            "Returned {Count} BillPayments. ElapsedMs={ElapsedMs}",
            paymentsList.Count,
            stopwatch.ElapsedMilliseconds
        );
        return Ok(paymentsList);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<BillPaymentDTO>> CreateBillPayment(CreateBillPaymentDTO request)
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
            return StatusCode(500, new { message = "Internal server error.", error = ex.Message });
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

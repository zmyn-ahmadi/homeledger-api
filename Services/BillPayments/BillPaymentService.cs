using HomeLedger.API.DTOs.BillPaymentsDTO;
using HomeLedger.API.Mappers;
using HomeLedger.API.Services.BillPayments;
using HomeLedger.API.Services.Correlation;

public class BillPaymentService(
    IUnitOfWork uow,
    BillPaymentMapper mapper,
    ILogger<BillPaymentService> logger,
    ICorrelationIdAccessor correlationIdAccessor
) : IBillPaymentService
{
    private readonly BillPaymentMapper _mapper = mapper;
    private readonly ILogger<BillPaymentService> _logger = logger;
    private readonly ICorrelationIdAccessor _correlationIdAccessor = correlationIdAccessor;

    public async Task<BillPaymentDTO> AddAsync(CreateBillPaymentDTO request)
    {
        // Retrieve the current CorrelationId (if any) using the shared helper.
        var correlationId = _correlationIdAccessor.GetCorrelationId();
        using var scope = _logger.BeginScope(
            new Dictionary<string, object>
            {
                ["CorrelationId"] = correlationId,
                ["BillId"] = request.BillId,
            }
        );
        _logger.LogInformation("Creating BillPayment for BillId {BillId}", request.BillId);

        try
        {
            var newPayment = _mapper.ToEntity(request);
            newPayment.CreatedAt = DateTime.UtcNow;
            newPayment.IsActive = true;

            await uow.BillPayments.AddAsync(newPayment);
            await uow.CommitAsync();

            _logger.LogInformation(
                "Created BillPayment {PaymentId} for BillId {BillId}",
                newPayment.BillPaymentId,
                request.BillId
            );

            return _mapper.ToDto(newPayment);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to create BillPayment for BillId {BillId}",
                request.BillId
            );
            throw ex;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var scope = _logger.BeginScope(
            new Dictionary<string, object>
            {
                // ["CorrelationId"] = _correlationIdAccessor.GetCorrelationId(),
                ["PaymentId"] = id,
            }
        );

        _logger.LogInformation("Deleting BillPayment {BillPaymentId}", id);

        var result = await uow.BillPayments.DeleteAsync(id);
        if (result)
        {
            await uow.CommitAsync();
            _logger.LogInformation("Deleted BillPayment {BillPaymentId}", id);
        }
        else
        {
            _logger.LogWarning("BillPayment {BillPaymentId} not found for deletion", id);
        }

        return result;
    }

    public async Task<IEnumerable<BillPaymentDTO>> GetAllAsync()
    {
        _logger.LogDebug("Fetching all BillPayments");
        var payments = await uow.BillPayments.GetAllAsync();
        var list = payments.Select(_mapper.ToDto).ToList();
        _logger.LogDebug("Fetched {Count} BillPayments", list.Count);
        return list;
    }

    public async Task<BillPaymentDTO> GetByIdAsync(int id)
    {
        _logger.LogDebug("Fetching BillPayment {BillPaymentId}", id);
        var payment = await uow.BillPayments.GetByIdAsync(id);

        if (payment is null)
            _logger.LogWarning("BillPaymentId {BillPaymentId} not found", id);

        return _mapper.ToDto(payment);
    }

    public async Task<bool> UpdateAsync(int id, UpdateBillPaymentDTO request)
    {
        using var scope = _logger.BeginScope(
            new Dictionary<string, object>
            {
                ["CorrelationId"] = _correlationIdAccessor.GetCorrelationId(),
                ["BillPaymentId"] = id,
            }
        );

        _logger.LogInformation("Updating BillPaymentId {BillPaymentId}", id);

        // Get existing
        var existingPayment = await uow.BillPayments.GetByIdAsync(id);
        if (existingPayment is null)
        {
            _logger.LogWarning("BillPaymentId {BillPaymentId} not found for update", id);
            throw new KeyNotFoundException($"Payment with ID {id} not found.");
        }

        // map the request to entity
        _mapper.UpdateEntity(request, existingPayment);
        existingPayment.UpdatedAt = DateTime.UtcNow;

        await uow.BillPayments.UpdateAsync(existingPayment);
        await uow.CommitAsync();

        _logger.LogInformation("Updated BillPaymentId {BillPaymentId}", id);

        return true;
    }
}

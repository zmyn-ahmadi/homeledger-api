using HomeLedger.API.DTOs.BillPaymentsDTO;
using HomeLedger.API.Mappers;
using HomeLedger.API.Services.BillPayments;

public class BillPaymentService(
    IUnitOfWork _uow,
    BillPaymentMapper mapper,
    ILogger<BillPaymentService> logger
) : IBillPaymentService
{
    private readonly BillPaymentMapper _mapper = mapper;
    private readonly ILogger<BillPaymentService> _logger = logger;

    public async Task<BillPaymentDTO> AddAsync(CreateBillPaymentDTO request)
    {
        using var scope = _logger.BeginScope(
            new Dictionary<string, object> { ["BillId"] = request.BillId }
        );

        var newPayment = _mapper.ToEntity(request);
        newPayment.CreatedAt = DateTime.UtcNow;
        newPayment.IsActive = true;

        await _uow.BillPayments.AddAsync(newPayment);
        await _uow.CommitAsync();

        _logger.LogInformation(
            "Created BillPayment {PaymentId} for BillId {BillId}",
            newPayment.BillPaymentId,
            request.BillId
        );

        return _mapper.ToDto(newPayment);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var scope = _logger.BeginScope(new Dictionary<string, object> { ["PaymentId"] = id });

        var result = await _uow.BillPayments.DeleteAsync(id);
        if (result)
        {
            await _uow.CommitAsync();
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
        var payments = await _uow.BillPayments.GetAllAsync();
        return [.. payments.Select(_mapper.ToDto)];
    }

    public async Task<BillPaymentDTO?> GetByIdAsync(int id)
    {
        var payment = await _uow.BillPayments.GetByIdAsync(id);

        if (payment is null)
        {
            _logger.LogWarning("BillPaymentId {BillPaymentId} not found", id);
            return null;
        }

        return _mapper.ToDto(payment);
    }

    public async Task<bool> UpdateAsync(int id, UpdateBillPaymentDTO request)
    {
        using var scope = _logger.BeginScope(
            new Dictionary<string, object> { ["BillPaymentId"] = id }
        );

        // Get existing
        var existingPayment = await _uow.BillPayments.GetByIdAsync(id);
        if (existingPayment is null)
        {
            _logger.LogWarning("BillPaymentId {BillPaymentId} not found for update", id);
            return false;
        }

        // map the request to entity
        _mapper.UpdateEntity(request, existingPayment);
        existingPayment.UpdatedAt = DateTime.UtcNow;

        await _uow.BillPayments.UpdateAsync(existingPayment);
        await _uow.CommitAsync();

        _logger.LogInformation("Updated BillPaymentId {BillPaymentId}", id);

        return true;
    }
}

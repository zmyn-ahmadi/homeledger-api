using HomeLedger.API.DTOs.BillPaymentsDTO;
using HomeLedger.API.Mappers;
using HomeLedger.API.Services.BillPayments;

public class BillPaymentService(IUnitOfWork uow, BillPaymentMapper mapper) : IBillPaymentService
{
    private readonly BillPaymentMapper _mapper = mapper;

    public async Task<BillPaymentDTO> AddAsync(CreateBillPaymentDTO request)
    {
        // Map the request to BillPayment object
        var NewPayment = _mapper.ToEntity(request);

        NewPayment.CreatedAt = DateTime.UtcNow;
        NewPayment.IsActive = true;

        // Add it to database
        await uow.BillPayments.AddAsync(NewPayment);
        // commit the change
        await uow.CommitAsync();

        // map the NewPayment to DTO
        return _mapper.ToDto(NewPayment);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var result = await uow.BillPayments.DeleteAsync(id);
        if (result)
            await uow.CommitAsync();
        return result;
    }

    public async Task<IEnumerable<BillPaymentDTO>> GetAllAsync()
    {
        var payments = await uow.BillPayments.GetAllAsync();
        return payments.Select(_mapper.ToDto);
    }

    public async Task<BillPaymentDTO> GetByIdAsync(int id)
    {
        var payment = await uow.BillPayments.GetByIdAsync(id);
        return _mapper.ToDto(payment);
    }

    public async Task<bool> UpdateAsync(int id, UpdateBillPaymentDTO request)
    {
        // Get existing
        var existingPayment =
            await uow.BillPayments.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Payment with ID {id} not found.");

        // map the request to entity
        _mapper.UpdateEntity(request, existingPayment);
        existingPayment.UpdatedAt = DateTime.UtcNow;

        await uow.BillPayments.UpdateAsync(existingPayment);
        await uow.CommitAsync();

        return true;
    }
}

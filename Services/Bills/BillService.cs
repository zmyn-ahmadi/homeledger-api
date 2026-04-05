using HomeLedger.API.DTOs.BillDTO;
using HomeLedger.API.Mappers;

public class BillService(IUnitOfWork uow, BillMapper mapper) : IBillService
{
    private readonly BillMapper _mapper = mapper;

    public async Task<BillDTO> AddAsync(CreateBillDTO request)
    {
        // map the req to Bill obj
        var newBill = _mapper.ToEntity(request);

        newBill.CreatedAt = DateTime.UtcNow;
        newBill.IsActive = true;

        await uow.Bills.AddAsync(newBill);
        await uow.CommitAsync();

        return _mapper.ToDto(newBill);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var result = await uow.Bills.DeleteAsync(id);
        if (result)
            await uow.CommitAsync();
        return result;
    }

    public async Task<IEnumerable<BillDTO>> GetAllAsync()
    {
        var bills = await uow.Bills.GetAllAsync();
        return bills.Select(_mapper.ToDto);
    }

    public async Task<BillDTO> GetByIdAsync(int id)
    {
        var bill = await uow.Bills.GetByIdAsync(id);
        return _mapper.ToDto(bill);
    }

    public async Task<bool> UpdateAsync(int id, UpdateBillDTO request)
    {
        var existingBill =
            await uow.Bills.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Bill with ID: {id} not found.");

        _mapper.UpdateEntity(request, existingBill);
        existingBill.UpdatedAt = DateTime.UtcNow;

        await uow.Bills.UpdateAsync(existingBill);
        await uow.CommitAsync();

        return true;
    }
}

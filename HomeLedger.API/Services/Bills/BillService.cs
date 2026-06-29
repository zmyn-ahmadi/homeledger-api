using HomeLedger.API.DTOs.BillDTO;
using HomeLedger.API.Mappers;
using HomeLedger.API.Utils.UserContext;

public class BillService(
    IUnitOfWork _uow,
    BillMapper mapper,
    ILogger<BillService> logger,
    IUserContext userContext
) : IBillService
{
    private readonly BillMapper _mapper = mapper;
    private readonly ILogger<BillService> _logger = logger;
    private readonly IUserContext _userCtx = userContext;

    public async Task<BillDTO> AddAsync(CreateBillDTO request)
    {
        var userId = _userCtx.UserId;

        using var scope = _logger.BeginScope(
            new Dictionary<string, object> { ["OwnerId"] = userId }
        );

        // map the req to Bill obj
        var newBill = _mapper.ToEntity(request);
        newBill.OwnerId = userId;
        newBill.CreatedAt = DateTime.UtcNow;
        newBill.IsActive = true;

        await _uow.Bills.AddAsync(newBill);
        await _uow.CommitAsync();

        _logger.LogInformation(
            "Created Bill {BillId} for user {OwnerId}",
            newBill.BillId,
            newBill.OwnerId
        );

        return _mapper.ToDto(newBill);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var scope = _logger.BeginScope(new Dictionary<string, object> { ["BillId"] = id });

        var result = await _uow.Bills.DeleteAsync(id);
        if (result)
        {
            await _uow.CommitAsync();
            _logger.LogInformation("Deleted Bill {BillId}", id);
        }
        else
        {
            _logger.LogWarning("Bill: {BillId} not found for deletion", id);
        }

        return result;
    }

    // FOR ADMIN ONLY
    public async Task<IEnumerable<BillDTO>> GetAllAsync()
    {
        var bills = await _uow.Bills.GetAllAsync();
        return [.. bills.Select(_mapper.ToDto)];
    }

    public async Task<IEnumerable<BillDTO>> GetAllBillByUserIdAsync()
    {
        var userId = _userCtx.UserId;

        using var scope = _logger.BeginScope(
            new Dictionary<string, object> { ["OwnerId"] = userId }
        );

        var bills = await _uow.Bills.GetAllByUserIdAsync(userId);
        return [.. bills.Select(_mapper.ToDto)];
    }

    public async Task<BillDTO?> GetByIdAsync(int id)
    {
        var bill = await _uow.Bills.GetByIdAsync(id);

        if (bill is null)
        {
            _logger.LogWarning("Bill: {BillId} not found.", id);
            return null;
        }

        return _mapper.ToDto(bill);
    }

    public async Task<bool> UpdateAsync(int id, UpdateBillDTO request)
    {
        using var scope = _logger.BeginScope(new Dictionary<string, object> { ["BillId"] = id });

        var existingBill = await _uow.Bills.GetByIdAsync(id);
        if (existingBill is null)
        {
            _logger.LogWarning("BillId not found for update", id);
            return false;
        }

        _mapper.UpdateEntity(request, existingBill);
        existingBill.UpdatedAt = DateTime.UtcNow;

        await _uow.Bills.UpdateAsync(existingBill);
        await _uow.CommitAsync();

        _logger.LogInformation("Updated Bill {BillId}", id);

        return true;
    }
}

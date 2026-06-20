using HomeLedger.API.DTOs.FamilyDTO;
using HomeLedger.API.Mappers;
using HomeLedger.API.Models;
using HomeLedger.API.Utils.UserContext;

namespace HomeLedger.API.Services.Family;

public class FamilyService(
    IUnitOfWork _uow,
    FamilyMapper mapper,
    ILogger<FamilyService> logger,
    IUserContext userContext
) : IFamilyService
{
    private readonly ILogger<FamilyService> _logger = logger;
    private readonly FamilyMapper _mapper = mapper;
    private readonly IUserContext _userCtx = userContext;

    public async Task<FamilyDTO?> GetByIdAsync(int id)
    {
        var fam = await _uow.Families.GetByIdAsync(id);
        if (fam is null)
        {
            _logger.LogWarning("Family: {FamilyId} not found.", id);
            return null;
        }

        return _mapper.ToDto(fam);
    }

    public async Task<IEnumerable<FamilyDTO>> GetAllAsync()
    {
        var fams = await _uow.Families.GetAllAsync();
        return [.. fams.Select(_mapper.ToDto)];
    }

    public async Task<FamilyDTO> AddAsync(CreateFamilyDTO request)
    {
        // get userId from jwt
        var userId = _userCtx.UserId;

        using var scope = _logger.BeginScope(
            new Dictionary<string, object> { ["OwnerId"] = userId }
        );

        // get user
        var user = await _uow.Users.GetByIdAsync(userId) ?? throw new Exception("User not found.");
        Console.WriteLine($"User: ${user}");

        // map
        var newFam = _mapper.ToEntity(request);

        // assign fields
        newFam.Users.Add(user);
        newFam.CreatedAt = DateTime.UtcNow;
        newFam.IsActive = true;

        await _uow.Families.AddAsync(newFam);
        await _uow.CommitAsync();

        _logger.LogInformation(
            "Created Family {FamilyId} by user {OwnerId}",
            newFam.FamilyId,
            userId
        );
        return _mapper.ToDto(newFam);
    }

    public async Task<FamilyDTO?> UpdateAsync(int id, UpdateFamilyDTO request)
    {
        using var scope = _logger.BeginScope(new Dictionary<string, object> { ["FamilyId"] = id });

        var existingFam = await _uow.Families.GetByIdAsync(id);
        if (existingFam is null)
        {
            _logger.LogWarning("Family: {FamilyId} not found for update", id);
            return null;
        }
        _mapper.UpdateEntity(request, existingFam);
        existingFam.UpdatedAt = DateTime.UtcNow;

        await _uow.Families.UpdateAsync(existingFam);
        await _uow.CommitAsync();

        _logger.LogInformation("Updated Family {FamilyId}", id);

        return _mapper.ToDto(existingFam);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var scope = _logger.BeginScope(new Dictionary<string, object> { ["FamilyId"] = id });

        var result = await _uow.Families.DeleteAsync(id);
        if (result)
        {
            await _uow.CommitAsync();
            _logger.LogInformation("Deleted Family {FamilyId}", id);
        }
        else
        {
            _logger.LogWarning("Family: {FamilyId} not found for deletion", id);
        }

        return result;
    }
}

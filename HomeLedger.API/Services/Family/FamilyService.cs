using System.Security.Cryptography;
using HomeLedger.API.DTOs.FamilyDTO;
using HomeLedger.API.Mappers;
using HomeLedger.API.Models;
using HomeLedger.API.Utils.UserContext;

namespace HomeLedger.API.Services.Family;

public class FamilyService(
    IUnitOfWork _uow,
    FamilyMapper mapper,
    ILogger<FamilyService> logger,
    IConfiguration config,
    IUserContext userContext
) : IFamilyService
{
    private readonly IConfiguration _config = config;
    private readonly ILogger<FamilyService> _logger = logger;
    private readonly FamilyMapper _mapper = mapper;
    private readonly IUserContext _userCtx = userContext;

    public async Task<FamilyDTO?> GetByIdAsync(int id)
    {
        // get userId from jwt
        var userId = _userCtx.UserId;

        using var scope = _logger.BeginScope(
            new Dictionary<string, object> { ["OwnerId"] = userId }
        );

        // get user
        var user = await _uow.Users.GetByIdAsync(userId) ?? throw new Exception("User not found.");

        var callerFamilyId = user.FamilyId;
        if (callerFamilyId != id)
        {
            _logger.LogWarning(
                "User {UserId} attempted t o access Family {FamilyId} -- denied.",
                _userCtx.UserId,
                id
            );
            return null;
        }

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

        // map
        var newFam = _mapper.ToEntity(request);

        // assign fields
        newFam.CreatedAt = DateTime.UtcNow;
        newFam.IsActive = true;

        await _uow.Families.AddAsync(newFam);
        await _uow.CommitAsync();

        user.Family = newFam;
        user.FamilyId = newFam.FamilyId;

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

    public async Task<string> InviteAsync()
    {
        // 1. Generate code
        var code = GenerateCode();

        // 2. build a Link
        var link = new Models.Link { Code = code };

        // 3. add to database
        await _uow.Links.AddAsync(link);
        await _uow.CommitAsync();

        // 3. return the generated link
        return $"{_config["App:BaseUrl"]}/invite?code={code}";
    }

    private string GenerateCode()
    {
        return Convert
            .ToBase64String(RandomNumberGenerator.GetBytes(32))
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");
    }
}

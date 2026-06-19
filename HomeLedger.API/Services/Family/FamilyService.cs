using HomeLedger.API.Models;
using HomeLedger.API.Utils.UserContext;

public class FamilyService(
    IUnitOfWork _uow,
    ILogger<FamilyService> logger,
    IUserContext userContext
) : IFamilyService
{
    private readonly ILogger<FamilyService> _logger = logger;
    private readonly IUserContext _userCtx = userContext;

    public async Task<Family?> GetByIdAsync(int id)
    {
        var fam = await _uow.Families.GetByIdAsync(id);
        if (fam is null)
        {
            _logger.LogWarning("Family: {FamilyId} not found.", id);
            return null;
        }

        return fam;
    }

    public async Task<IEnumerable<Family>> GetAllAsync()
    {
        var fams = await _uow.Families.GetAllAsync();
        return fams;
    }

    public async Task<Family> AddAsync(Family request)
    {
        var userId = _userCtx.UserId;
        using var scope = _logger.BeginScope(
            new Dictionary<string, object> { ["OwnerId"] = userId }
        );

        var newFam = new Family();
        var user = await _uow.Users.GetByIdAsync(userId);

        newFam.Users.Add(user!);

        await _uow.Families.AddAsync(newFam);
        await _uow.CommitAsync();

        _logger.LogInformation(
            "Created Family {FamilyId} by user {OwnerId}",
            newFam.FamilyId,
            userId
        );
        return newFam;
    }

    public async Task<Family> UpdateAsync(int id, Family request)
    {
        throw new NotImplementedException();
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

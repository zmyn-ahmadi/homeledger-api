using HomeLedger.API.DTOs.LinkDTO;
using HomeLedger.API.Utils.UserContext;

namespace HomeLedger.API.Services.Link;

public class LinkService(
    IUserContext userContext,
    IUnitOfWork _uow,
    ILogger<LinkService> logger,
    LinkMapper mapper
) : ILinkService
{
    private readonly IUserContext _userCtx = userContext;
    private readonly ILogger<LinkService> _logger = logger;
    private readonly LinkMapper _mapper = mapper;

    public async Task<LinkDTO?> GetLinkAsyncByUserId(int id)
    {
        return new LinkDTO { };
    }

    public async Task<LinkDTO> AddLinkAsync(CreateLinkDTO req)
    {
        var userId = _userCtx.UserId;

        var currentUser =
            await _uow.Users.GetByIdAsync(userId) ?? throw new Exception("User not found");

        using var scope = _logger.BeginScope(
            new Dictionary<string, object> { ["OwnerId"] = userId }
        );

        var newLink = _mapper.ToEntity(req);
        newLink.IsRevoked = false;
        var expiration_time = DateTime.UtcNow.AddMinutes(5);
        newLink.ExpiresAt = expiration_time;
        newLink.MaxUses = 1;
        newLink.UseCount = 0;
        newLink.CreatedBy = userId;
        newLink.FamilyId = currentUser.FamilyId;

        await _uow.Links.AddAsync(newLink);
        await _uow.CommitAsync();

        return _mapper.ToDto(newLink);
    }

    public async Task<bool> DeleteLinkAsync(int id)
    {
        return false;
    }
}

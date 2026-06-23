using HomeLedger.API.DTOs.LinkDTO;

namespace HomeLedger.API.Services.Link;

public interface ILinkService
{
    Task<LinkDTO?> GetLinkAsyncByUserId(int id);
    Task<LinkDTO> AddLinkAsync(CreateLinkDTO req);
    Task<bool> DeleteLinkAsync(int id);
}

using HomeLedger.API.DTOs.FamilyDTO;
using HomeLedger.API.Models;

namespace HomeLedger.API.Services.Family;

public interface IFamilyService
{
    Task<IEnumerable<FamilyDTO>> GetAllAsync();
    Task<FamilyDTO?> GetByIdAsync(int id);
    Task<FamilyDTO> AddAsync(CreateFamilyDTO dto);
    Task<FamilyDTO?> UpdateAsync(int id, UpdateFamilyDTO dto);
    Task<bool> DeleteAsync(int id);
    Task<string> InviteAsync();
}

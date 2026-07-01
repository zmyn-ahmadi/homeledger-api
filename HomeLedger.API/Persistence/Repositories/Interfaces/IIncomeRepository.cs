using HomeLedger.API.DTOs.FamilyDTO;
using HomeLedger.API.Models;
using HomeLedger.API.Persistence.Repositories.Interfaces;

public interface IIncomeRepository : IRepository<Income>
{
    Task<List<Income>> GetAllByUserIdAsync(int userId);

    // Member's income
    Task<List<FamilyIncomeDTO>> GetMemberIncomesAsync(int familyId);
}

using HomeLedger.API.Models;
using HomeLedger.API.Persistence.Repositories.Interfaces;

public interface IFamilyMemberRepository : IRepository<FamilyMember>
{
    Task<FamilyMember?> GetByUserIdAsync(int userId);
    Task<FamilyMember?> GetByUserIdAndFamilyIdAsync(int userId, int familyId);
}

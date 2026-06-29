using HomeLedger.API.Models;
using HomeLedger.API.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HomeLedger.API.Persistence.Repositories;

public class FamilyMemberRepository(HomeLedgerDbContext context)
    : Repository<FamilyMember>(context),
        IFamilyMemberRepository
{
    public async Task<FamilyMember?> GetByUserIdAndFamilyIdAsync(int userId, int familyId)
    {
        return await _dbSet.FirstOrDefaultAsync(x => x.UserId == userId && x.FamilyId == familyId);
    }

    public async Task<FamilyMember?> GetByUserIdAsync(int userId)
    {
        return await _dbSet.FirstOrDefaultAsync(x => x.UserId == userId);
    }
}

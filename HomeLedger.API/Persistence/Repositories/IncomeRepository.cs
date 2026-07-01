using HomeLedger.API.DTOs.FamilyDTO;
using HomeLedger.API.Models;
using HomeLedger.API.Persistence;
using Microsoft.EntityFrameworkCore;

public class IncomeRepository(HomeLedgerDbContext context)
    : Repository<Income>(context),
        IIncomeRepository
{
    public async Task<List<Income>> GetAllByUserIdAsync(int userId) =>
        await _dbSet
            .Include(i => i.Category)
            .Where(i => i.OwnerId == userId)
            .ToListAsync();

    public async Task<List<FamilyIncomeDTO>> GetMemberIncomesAsync(int familyId)
    {
        return await _dbSet
            .Where(i =>
                i.Owner != null && i.Owner.FamilyMemberships.Any(fm => fm.FamilyId == familyId)
            )
            .OrderByDescending(i => i.Date)
            .Select(i => new FamilyIncomeDTO
            {
                IncomeId = i.IncomeId,
                Amount = i.Amount,
                Date = i.Date,
                Description = i.Description,
                Note = i.Note,
                UserId = i.OwnerId,
                FullName = i.Owner != null ? i.Owner.FullName : null,
            })
            .ToListAsync();
    }
}

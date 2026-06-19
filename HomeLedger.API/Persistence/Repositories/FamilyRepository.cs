using HomeLedger.API.Models;
using HomeLedger.API.Persistence;
using HomeLedger.API.Persistence.Repositories.Interfaces;

namespace HomeLedger.API.Persistence.Repositories;

public class FamilyRepository(HomeLedgerDbContext context)
    : Repository<Family>(context),
        IFamilyRepository { }

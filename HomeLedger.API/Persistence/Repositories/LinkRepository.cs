using HomeLedger.API.Models;
using HomeLedger.API.Persistence;
using HomeLedger.API.Persistence.Repositories.Interfaces;

namespace HomeLedger.API.Persistence.Repositories;

public class LinkRepository(HomeLedgerDbContext context)
    : Repository<Link>(context),
        ILinkRepository { }

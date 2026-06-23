using HomeLedger.API.Models;

namespace HomeLedger.API.Persistence.Repositories.Interfaces;

public interface ILinkRepository : IRepository<Link>
{
    Task<Link?> GetLinkByCode(string code);
}

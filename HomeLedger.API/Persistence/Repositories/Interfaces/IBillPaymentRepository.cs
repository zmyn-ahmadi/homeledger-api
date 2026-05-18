using HomeLedger.API.Models;

namespace HomeLedger.API.Persistence.Repositories.Interfaces;

public interface IBillPaymentRepository : IRepository<BillPayment>
{
    new Task<IEnumerable<BillPayment>> GetAllAsync();
}

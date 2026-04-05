using HomeLedger.API.Models;
using HomeLedger.API.Persistence;

namespace HomeLedger.API.Persistence.Repositories;

public class BillPaymentRepository(HomeLedgerDbContext context)
    : Repository<BillPayment>(context) { }

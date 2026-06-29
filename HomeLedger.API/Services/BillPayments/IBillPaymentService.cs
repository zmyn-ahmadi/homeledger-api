using HomeLedger.API.DTOs.BillPaymentsDTO;

namespace HomeLedger.API.Services.BillPayments;

public interface IBillPaymentService
{
    Task<IEnumerable<BillPaymentDTO>> GetAllAsync();
    Task<BillPaymentDTO?> GetByIdAsync(int id);
    Task<BillPaymentDTO> AddAsync(CreateBillPaymentDTO request);
    Task<bool> UpdateAsync(int id, UpdateBillPaymentDTO request);
    Task<bool> DeleteAsync(int id);
    Task<List<BillPaymentDTO>> GetAllByUserIdAsync();
}

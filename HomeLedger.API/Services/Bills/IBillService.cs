using HomeLedger.API.DTOs.BillDTO;

public interface IBillService
{
    Task<IEnumerable<BillDTO>> GetAllAsync();
    Task<BillDTO?> GetByIdAsync(int id);
    Task<BillDTO> AddAsync(CreateBillDTO request);
    Task<bool> UpdateAsync(int id, UpdateBillDTO request);
    Task<bool> DeleteAsync(int id);
}

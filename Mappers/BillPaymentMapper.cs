using HomeLedger.API.DTOs.BillDTO;
using HomeLedger.API.DTOs.BillPaymentsDTO;
using HomeLedger.API.DTOs.UserDTO;
using HomeLedger.API.Models;
using Riok.Mapperly.Abstractions;

namespace HomeLedger.API.Mappers;

[Mapper]
public partial class BillPaymentMapper
{
    public partial BillPaymentDTO ToDto(BillPayment billPayment);

    public partial BillPayment ToEntity(CreateBillPaymentDTO dto);

    public partial void UpdateEntity(UpdateBillPaymentDTO dto, BillPayment entity);

    private partial BillDTO MapBill(Bill bill);

    private partial UserDTO MapUser(User user);
}

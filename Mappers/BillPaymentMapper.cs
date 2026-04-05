using HomeLedger.API.DTOs.BillPaymentsDTO;
using HomeLedger.API.Models;
using Riok.Mapperly.Abstractions;

namespace HomeLedger.API.Mappers;

[Mapper]
public partial class BillPaymentMapper
{
    public partial BillPaymentDTO ToDto(BillPayment billPayment);

    public partial BillPaymentDTO UpdateEntity(UpdateBillPaymentDTO dto, BillPayment entity);

    public partial BillPayment ToEntity(CreateBillPaymentDTO dto);
}

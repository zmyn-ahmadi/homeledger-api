using HomeLedger.API.DTOs.BillDTO;
using HomeLedger.API.Models;
using Riok.Mapperly.Abstractions;

namespace HomeLedger.API.Mappers;

[Mapper]
public partial class BillMapper
{
    public partial BillDTO ToDto(Bill entity);

    public partial void UpdateEntity(UpdateBillDTO dto, Bill entity);

    public partial Bill ToEntity(CreateBillDTO dto);
}

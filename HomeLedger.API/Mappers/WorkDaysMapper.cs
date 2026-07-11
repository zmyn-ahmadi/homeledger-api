using HomeLedger.API.DTOs.WorkDaysDTO;
using Riok.Mapperly.Abstractions;

namespace HomeLedger.API.Mappers
{
    [Mapper]
    public partial class WorkDaysMapper
    {
        public partial WorkDaysDTO ToDto(Models.WorkDays workDays);
    }
}

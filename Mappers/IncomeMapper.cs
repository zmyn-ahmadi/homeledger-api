using HomeLedger.API.DTOs.IncomeDTO;
using HomeLedger.API.Models;
using Riok.Mapperly.Abstractions;

namespace HomeLedger.API.Mappers
{
    [Mapper]
    public partial class IncomeMapper
    {
        public partial IncomeDTO ToDto(Income income);

        public partial Income ToEntity(CreateIncomeDTO dto);

        public partial IncomeDTO UpdateEntity(UpdateIncomeDTO dto, Income entity);

        public partial IncomeDTO CreateEntity(CreateIncomeDTO dto);
    }
}

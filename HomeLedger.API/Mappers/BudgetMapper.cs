using HomeLedger.API.DTOs.BudgetDTO;
using HomeLedger.API.Models;
using Riok.Mapperly.Abstractions;

namespace HomeLedger.API.Mappers;

/**
 * This class contains the Budget Data Transfer Object methods.
 * Methods include converting Budget entities to DTOs, updating entities with DTOs, and converting DTOs to entities.
 */

[Mapper]
public partial class BudgetMapper
{
    public partial BudgetDTO ToDto(Budget entity);

    public partial BudgetDTO UpdateEntity(UpdateBudgetDTO dto, Budget entity);

    public partial Budget ToEntity(CreateBudgetDTO dto);
}

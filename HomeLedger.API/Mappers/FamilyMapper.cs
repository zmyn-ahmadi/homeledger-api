using HomeLedger.API.DTOs.FamilyDTO;
using HomeLedger.API.Models;
using Riok.Mapperly.Abstractions;

namespace HomeLedger.API.Mappers;

[Mapper]
public partial class FamilyMapper
{
    public partial FamilyDTO ToDto(Family family);

    public partial Family ToDomain(FamilyDTO familyDto);

    // CreateFamilyDTO -> Family
    public partial Family ToEntity(CreateFamilyDTO dto);

    public partial void UpdateEntity(UpdateFamilyDTO dto, Family entity);

    public partial FamilyDTO CreateEntity(FamilyDTO dto);
}


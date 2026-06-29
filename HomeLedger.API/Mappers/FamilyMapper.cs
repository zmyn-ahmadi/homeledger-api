using HomeLedger.API.DTOs.FamilyDTO;
using HomeLedger.API.DTOs.FamilyMemberDTO;
using HomeLedger.API.Models;
using Riok.Mapperly.Abstractions;

namespace HomeLedger.API.Mappers;

[Mapper]
public partial class FamilyMapper
{
    // Read
    public partial FamilyDTO ToDto(Family family);

    // CreateFamilyDTO → new Family
    [MapperIgnoreTarget(nameof(Family.Members))]
    [MapperIgnoreTarget(nameof(Family.InviteLinks))]
    [MapperIgnoreTarget(nameof(Family.Bills))]
    [MapperIgnoreTarget(nameof(Family.Payments))]
    [MapperIgnoreTarget(nameof(Family.Expenses))]
    [MapperIgnoreTarget(nameof(Family.Incomes))]
    [MapperIgnoreTarget(nameof(Family.Budgets))]
    [MapperIgnoreTarget(nameof(Family.FamilyId))]
    public partial Family ToEntity(CreateFamilyDTO dto);

    // UpdateFamilyDTO → patch existing entity
    [MapperIgnoreTarget(nameof(Family.Members))]
    [MapperIgnoreTarget(nameof(Family.InviteLinks))]
    [MapperIgnoreTarget(nameof(Family.Bills))]
    [MapperIgnoreTarget(nameof(Family.Payments))]
    [MapperIgnoreTarget(nameof(Family.Expenses))]
    [MapperIgnoreTarget(nameof(Family.Incomes))]
    [MapperIgnoreTarget(nameof(Family.Budgets))]
    [MapperIgnoreTarget(nameof(Family.FamilyId))]
    public partial void UpdateEntity(UpdateFamilyDTO dto, Family entity);

    public partial FamilyMemberDTO ToDto(FamilyMember member);
}

using HomeLedger.API.DTOs.FamilyDTO;
using HomeLedger.API.Models;
using Riok.Mapperly.Abstractions;

namespace HomeLedger.API.Mappers;

[Mapper]
public partial class FamilyMapper
{
    // ── Read ────────────────────────────────────────────────────────────────

    public FamilyDTO ToDto(Family family) =>
        new()
        {
            FamilyId = family.FamilyId,
            Name = family.Name,
            CreatedAt = family.CreatedAt,
            Users =
            [
                .. family.Users.Select(u => new FamilyUserDTO
                {
                    UserId = u.UserId,
                    DisplayName = u.FullName ?? string.Empty,
                    Email = u.Email,
                }),
            ],
            Bills =
            [
                .. family.Bills.Select(b => new FamilyBillDTO
                {
                    BillId = b.BillId,
                    Name = b.Name,
                    Amount = b.Amount,
                }),
            ],
        };

    // ── Write ───────────────────────────────────────────────────────────────

    // CreateFamilyDTO → new Family (no Users — EF handles membership separately)
    [MapperIgnoreTarget(nameof(Family.Users))]
    [MapperIgnoreTarget(nameof(Family.Bills))]
    [MapperIgnoreTarget(nameof(Family.Payments))]
    [MapperIgnoreTarget(nameof(Family.Expenses))]
    [MapperIgnoreTarget(nameof(Family.Incomes))]
    [MapperIgnoreTarget(nameof(Family.Budgets))]
    [MapperIgnoreTarget(nameof(Family.FamilyId))] // DB-generated
    public partial Family ToEntity(CreateFamilyDTO dto);

    // UpdateFamilyDTO → patch existing entity in place
    [MapperIgnoreTarget(nameof(Family.Users))]
    [MapperIgnoreTarget(nameof(Family.Bills))]
    [MapperIgnoreTarget(nameof(Family.Payments))]
    [MapperIgnoreTarget(nameof(Family.Expenses))]
    [MapperIgnoreTarget(nameof(Family.Incomes))]
    [MapperIgnoreTarget(nameof(Family.Budgets))]
    [MapperIgnoreTarget(nameof(Family.FamilyId))]
    public partial void UpdateEntity(UpdateFamilyDTO dto, Family entity);
}

using HomeLedger.API.DTOs.LinkDTO;
using HomeLedger.API.Models;
using Riok.Mapperly.Abstractions;

[Mapper]
public partial class LinkMapper
{
    public LinkDTO ToDto(Link link) =>
        new()
        {
            LinkId = link.LinkId,
            Code = link.Code,
            IsRevoked = link.IsRevoked,
            ExpiresAt = link.ExpiresAt,
            MaxUses = link.MaxUses,
            UseCount = link.UseCount,
            FamilyId = link.FamilyId,
            CreatedBy = link.CreatedBy,
        };

    public partial Link ToEntity(CreateLinkDTO dto);
}

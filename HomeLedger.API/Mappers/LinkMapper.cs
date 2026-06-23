using HomeLedger.API.DTOs.LinkDTO;
using HomeLedger.API.Models;
using Riok.Mapperly.Abstractions;

namespace HomeLedger.API.Mappers;

[Mapper]
public partial class LinkMapper
{
    public partial LinkDTO ToDto(Link entity);

    public partial Link ToEntity(CreateLinkDTO dto);
}

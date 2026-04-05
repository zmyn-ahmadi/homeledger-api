using HomeLedger.API.DTOs.UserDTO;
using HomeLedger.API.Models;
using Riok.Mapperly.Abstractions;

namespace HomeLedger.API.Mappers;

[Mapper]
public partial class UserMapper
{
    // User -> UserDTO
    // Tell Mapperly to ignore FullName — we'll set it manually
    [MapperIgnoreTarget(nameof(UserDTO.FullName))]
    private partial UserDTO ToDtoInternal(User user);

    public UserDTO ToDto(User user)
    {
        var dto = ToDtoInternal(user);
        dto.FullName = $"{user.FirstName ?? ""} {user.LastName ?? ""}".Trim();
        return dto;
    }

    // CreateDTO -> User
    public partial User ToEntity(CreateUserDTO dto);

    // UpdateDTO -> User
    public partial void UpdateEntity(UpdateUserDTO dto, User entity);
}

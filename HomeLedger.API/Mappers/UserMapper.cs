using HomeLedger.API.DTOs.UserDTO;
using HomeLedger.API.Models;
using Riok.Mapperly.Abstractions;

namespace HomeLedger.API.Mappers;

[Mapper]
public partial class UserMapper
{
    // User -> UserDTO
    public partial UserDTO ToDto(User user);

    // CreateDTO -> User
    public partial User ToEntity(CreateUserDTO dto);

    // UpdateDTO -> User
    public partial void UpdateEntity(UpdateUserDTO dto, User entity);
}

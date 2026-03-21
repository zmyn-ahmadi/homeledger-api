using AutoMapper;
using HomeLedger.API.DTOs.User;
using HomeLedger.API.Models;

namespace HomeLedger.API.MappingProfiles;

public class UserMappingProfile : Profile
{
  public UserMappingProfile()
  {
    CreateMap<User, UserDTO>()
        .ForMember(
            dest => dest.FullName,
            opt => opt.MapFrom(src => $"{src.FirstName ?? ""} {src.LastName ?? ""}".Trim())
        );

    CreateMap<CreateUserDTO, User>();
    CreateMap<UpdateUserDTO, User>();
  }
}

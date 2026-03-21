using AutoMapper;
using HomeLedger.API.DTOs.Category;
using HomeLedger.API.Models;

namespace HomeLedger.API.MappingProfiles;

public class CategoryMappingProfile : Profile
{
  public CategoryMappingProfile()
  {
    CreateMap<Category, CategoryDTO>();
    CreateMap<UpdateCategoryDTO, Category>();
    CreateMap<CreateCategoryDTO, Category>()
        .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name.Trim().ToLower()));
  }
}

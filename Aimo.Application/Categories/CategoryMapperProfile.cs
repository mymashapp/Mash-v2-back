using Aimo.Core.Infrastructure;
using Aimo.Domain.Categories;
using AutoMapper;

namespace Aimo.Application.Categories;


internal class CategoryMapperProfile : Profile, IOrderedMapperProfile
{
    public int Order => 1;

    public CategoryMapperProfile()
    {
        CreateMap<Category, CategoryDto>().ReverseMap();
        CreateMap<SubCategory, SubCategoryDto>().ReverseMap();
        
        CreateMap<SubCategory, SubCategoryWithCategoryDto>()
            .ForMember(d => d.CategoryName, e => e.MapFrom(x => x.Category.Name)) 
            .ReverseMap();
        
        CreateMap<Category, SubCategoryWithCategoryDto>()
            .ForMember(d => d.Alias, e => e.MapFrom(x => x.SubCategories.Select(x=>x.Alias))) 
            .ForMember(d => d.CategoryName, e => e.MapFrom(x => x.Name)) 
            .ReverseMap();
    }
}
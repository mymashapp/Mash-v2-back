using Aimo.Core.Infrastructure;
using Aimo.Domain.Category;
using AutoMapper;

namespace Aimo.Application.Category;

internal class CategoryMapperProfile : Profile, IOrderedMapperProfile
{
    public int Order => 1;

    public CategoryMapperProfile()
    {
        CreateMap<Domain.Category.Category, CategoryDto>().ReverseMap();
    }
}
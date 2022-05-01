using Aimo.Core.Infrastructure;
using Aimo.Domain.Interests;
using AutoMapper;

namespace Aimo.Application.Interests;

internal class InterestMapperProfile : Profile, IOrderedMapperProfile
{
    public int Order => 1;

    public InterestMapperProfile()
    {
        CreateMap<Interest, InterestDto>().ReverseMap();
    }
}
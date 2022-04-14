using Aimo.Core.Infrastructure;
using Aimo.Domain.Users;
using AutoMapper;

namespace Aimo.Application.Users;

internal class UserMapperProfile : Profile, IOrderedMapperProfile
{
    public int Order => 1;

    public UserMapperProfile()
    {
        CreateMap<User, UserDto>().ReverseMap();
        CreateMap<UserPreference, UserPreferencesDto>().ReverseMap();
        CreateMap<Interest, UserInterestDto>().ReverseMap();
        CreateMap<Picture, UserPhotoDto>().ReverseMap();
        CreateMap<Picture, PictureDto>()
            .ForMember(d => d.Picture, e => e.MapFrom(x => Convert.ToBase64String(x.Binary)))
            .ReverseMap()
            .ForMember(e => e.Binary, d => d.MapFrom(x => Convert.FromBase64String(x.Picture)));
    }
}
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
    }
}
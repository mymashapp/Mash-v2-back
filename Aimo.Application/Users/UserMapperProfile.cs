using Aimo.Core;
using Aimo.Core.Infrastructure;
using Aimo.Domain.Users;
using AutoMapper;

namespace Aimo.Application.Users;

internal class UserMapperProfile : Profile, IOrderedMapperProfile
{
    public int Order => 1;

    public UserMapperProfile()
    {
        CreateMap<User, UserViewDto>().ReverseMap();
        CreateMap<User, UserSaveDto>().ReverseMap();
           
        CreateMap<Interest, IdNameDto>().ReverseMap();

        CreateMap<Interest, UserInterestDto>().ReverseMap();
        CreateMap<Picture, UserPhotoViewDto>().ReverseMap();
        CreateMap<Picture, PictureDto>()
            .ForMember(d => d.Picture, e => e.MapFrom(x => Convert.ToBase64String(x.Binary)))
            .ReverseMap()
            .ForMember(e => e.Binary, d => d.MapFrom(x => Convert.FromBase64String(x.Picture)));
    }
}
using Aimo.Core.Infrastructure;
using Aimo.Domain.Interests;
using Aimo.Domain.Users;
using Aimo.Domain.Users.Entities;
using AutoMapper;

namespace Aimo.Application.Users;

internal class UserMapperProfile : Profile, IOrderedMapperProfile
{
    public int Order => 1;

    public UserMapperProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(d => d.UploadedPictures, opt => opt.Ignore())
            .ForMember(d => d.Pictures, e =>
                e.MapFrom(x => x.Pictures))
            .ReverseMap()
            .ForMember(d => d.Interests, opt => opt.Ignore())
            .ForMember(e => e.Pictures, opt => opt.Ignore());

        CreateMap<Interest, IdNameDto>().ReverseMap();

        CreateMap<Interest, UserInterestDto>()
            .ForMember(d => d.InterestName,
                e => e.MapFrom(x => x.Name))
            .ReverseMap();
        CreateMap<Picture, UserPictureDto>()
            .ForMember(d => d.PictureUrl,
                e => e.MapFrom(x => x.Url))
            .ReverseMap();
    }
}
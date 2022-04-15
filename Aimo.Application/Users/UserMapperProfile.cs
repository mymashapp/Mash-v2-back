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
        CreateMap<User, UserDto>()
            .ForMember(d => d.PicturesBase64, opt => opt.Ignore())
            .ForMember(d => d.Pictures, e => e.MapFrom(x => x.Pictures))
            .ReverseMap()
            .ForMember(e => e.Pictures, opt => opt.Ignore());

        CreateMap<Interest, IdNameDto>().ReverseMap();

        CreateMap<Interest, UserInterestDto>()
            .ForMember(d => d.InterestName, e => e.MapFrom(x => x.Name))
            .ReverseMap();
        CreateMap<Picture, UserPhotoDto>().ReverseMap();
        CreateMap<Picture, PictureDto>()
            // .ForMember(d => d.Picture, e => e.MapFrom(x => Convert.ToBase64String(x.Binary)))
            .ReverseMap();
        //.ForMember(e => e.Binary, d => d.MapFrom(x => Convert.FromBase64String(x.Picture)));
        
        
    }
}
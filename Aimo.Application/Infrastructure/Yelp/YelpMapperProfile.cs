using Aimo.Core.Infrastructure;
using Aimo.Domain.Cards;
using Aimo.Domain.Categories;
using Aimo.Domain.Users;
using AutoMapper;

namespace Aimo.Data.Infrastructure.Yelp;

internal class YelpMapperProfile : Profile, IOrderedMapperProfile
{
    public int Order => 1;

    public YelpMapperProfile()
    {
        CreateMap<Business, YelpCardDto>()
            .ForMember(d => d.Latitude, e => e.MapFrom(x => x.Coordinates.Latitude))
            .ForMember(d => d.Longitude, e => e.MapFrom(x => x.Coordinates.Longitude))
            .ForMember(d => d.ZipCode, e => e.MapFrom(x => x.Location.ZipCode))
            .ForMember(d => d.Address1, e => e.MapFrom(x => x.Location.Address1))
            .ForMember(d => d.Address2, e => e.MapFrom(x => x.Location.Address2))
            .ForMember(d => d.Address3, e => e.MapFrom(x => x.Location.Address3))
            .ForMember(d => d.City, e => e.MapFrom(x => x.Location.City))
            .ForMember(d => d.State, e => e.MapFrom(x => x.Location.State))
            .ForMember(d => d.Country, e => e.MapFrom(x => x.Location.Country))
            .ForMember(d => d.CardType, e =>e.MapFrom(x => CardType.Yelp));
        
        CreateMap<YelpCardDto, Card>()
            .ForMember(t=>t.SubCategories,i=>i.Ignore())
            .ForMember(t=>t.Category,i=>i.Ignore());
        
        CreateMap<YelpSubCategory,SubCategory>();
        CreateMap<YelpRawResponsePicture, CardPictureDto>()
            .ForMember(e => e.PictureUrl, d => d.MapFrom(x => x.CardPicture.Select(x => x)));

        CreateMap<YelpCardDto, Card>()
            .ForMember(t => t.SubCategories, i => i.Ignore())
            .ForMember(t => t.Category, i => i.Ignore());
    }
}
using Aimo.Core.Infrastructure;
using Aimo.Domain.Cards;
using Aimo.Domain.Users;
using AutoMapper;

namespace Aimo.Data.Infrastructure.Yelp;

internal class CardMapperProfile : Profile, IOrderedMapperProfile
{
    public int Order => 1;

    public CardMapperProfile()
    {
        CreateMap<Card, CardListDto>()
            .ForMember(d => d.Address,
                e => e.MapFrom(x => $"{x.Address1}, {x.Address2}, {x.Address3}, {x.City}, {x.State}, {x.ZipCode}"))
            .ForMember(t => t.Category, e => e.MapFrom(x => x.Category.Name))
            .ForMember(t => t.SubCategories, e => e.MapFrom(x => x.SubCategories.Select(s => s.Title)));
        

        CreateMap<YelpCardDto, Card>()
            .ForMember(t => t.SubCategories, i => i.Ignore())
            .ForMember(t => t.Category, i => i.Ignore());

        CreateMap<SubCategory, Aimo.Domain.Categories.SubCategory>();
    }
}
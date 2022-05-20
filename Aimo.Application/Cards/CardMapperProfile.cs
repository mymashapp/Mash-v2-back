using Aimo.Core.Infrastructure;
using Aimo.Data.Infrastructure.Yelp;
using Aimo.Domain.Cards;
using Aimo.Domain.Categories;
using Aimo.Domain.Users;
using AutoMapper;

namespace Aimo.Application.Cards;

internal class CardMapperProfile : Profile, IOrderedMapperProfile
{
    public int Order => 1;

    public CardMapperProfile()
    {
        CreateMap<Card, CardDto>()
            .ForMember(d => d.Address,
                e => e.MapFrom(x => new string[] {x.Address1, x.Address2, x.Address3, x.City, x.State, x.ZipCode}.JoinToString(",")))
            .ReverseMap()
            .ForMember(d => d.Address1, e => e.MapFrom(x =>x.Address))
            .ForMember(e => e.PictureUrl, opt => opt.Condition(d => d.PictureUrl.IsNotEmpty()))
            .AfterMap((d, e) =>
            {
                switch (d.CardType)
                {
                    case CardType.Own:
                    {
                        e.Alias = e.Alias.IsNullOrWhiteSpace() ? $"{d.Name}-{Guid.NewGuid()}" : e.Alias;
                        e.Url = string.Empty;
                        break;
                    }
                }
            });

        CreateMap<Card, CardPictureDto>()
            .ForMember(e => e.CardId, d => d.MapFrom(x => x.Id));


        CreateMap<CardPictureDto, CardPicture>()
            .ReverseMap()
            .ForMember(e => e.PictureUrl, opt => opt.Condition(d => d.PictureUrl.IsNotEmpty()));

        CreateMap<Card, CardListDto>()
            .ForMember(d => d.Address,
                e => e.MapFrom(x => $"{x.Address1}, {x.Address2}, {x.Address3}, {x.City}, {x.State}, {x.ZipCode}"))
            .ForMember(t => t.Category, e => e.MapFrom(x => x.Category.Name))
            .ForMember(t => t.SubCategories, e => e.MapFrom(x => x.SubCategories.Select(s => s.Title)))
            .ForMember(t => t.Pictures, e => e.MapFrom(x => x.CardPictures.Select(s => s.PictureUrl))).ReverseMap();


        CreateMap<YelpSubCategory, SubCategory>();
        CreateMap<CardQuerySelector, CardListDto>()
            .IncludeMembers(x => x.Card).ForMember(t => t.SwipeCount,
                e => e.MapFrom(x => x.SwipeCount != null ? x.SwipeCount : 0));
    }
}
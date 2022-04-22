using Aimo.Core.Infrastructure;
using Aimo.Domain.Cards;
using AutoMapper;

namespace Aimo.Application.Cards;

internal class CardMapperProfile : Profile, IOrderedMapperProfile
{
    public int Order => 1;

    public CardMapperProfile()
    {
        CreateMap<Card, CardDto>()
            .ReverseMap()
            .ForMember(e => e.PictureUrl, opt => opt.Condition(d => d.PictureUrl.IsNotEmpty()));
    }
}
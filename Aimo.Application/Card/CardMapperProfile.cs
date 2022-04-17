using Aimo.Core.Infrastructure;
using Aimo.Domain.Card;
using AutoMapper;

namespace Aimo.Application.Card;

internal class CardMapperProfile : Profile, IOrderedMapperProfile
{
    public int Order => 1;

    public CardMapperProfile()
    {
        CreateMap<Domain.Card.Card, CardDto>().ReverseMap();
    }
}
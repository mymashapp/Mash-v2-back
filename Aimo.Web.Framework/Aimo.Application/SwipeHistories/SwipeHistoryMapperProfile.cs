using Aimo.Core.Infrastructure;
using Aimo.Domain.Chats;
using Aimo.Domain.SwipeHistories;
using Aimo.Domain.Users.Entities;
using AutoMapper;

namespace Aimo.Application.SwipeHistories;

internal class SwipeHistoryMapperProfile : Profile, IOrderedMapperProfile
{
    public int Order => 1;

    public SwipeHistoryMapperProfile()
    {
        CreateMap<SwipeHistory, SwipeHistoryDto>().ReverseMap();


        CreateMap<User, SwipeGroupDto>()
            .ForMember(d => d.UserId, e => e.MapFrom(x => x.Id))
            .ForMember(d => d.AgeFrom, e => e.MapFrom(x => x.PreferenceAgeFrom))
            .ForMember(d => d.AgeTo, e => e.MapFrom(x => x.PreferenceAgeTo))
            .ForMember(d => d.GroupType, e => e.MapFrom(x => x.PreferenceGroupOf))
            .ForMember(d => d.Id, o => o.Ignore());

        CreateMap<SwipeGroup, SwipeGroupDto>().ReverseMap(); 

        CreateMap<SwipeGroup, SwipeGroupInterestDto>()
            .ForMember(d => d.SwipeGroupId, e => e.MapFrom(x => x.Id))
            .ReverseMap();

        #region chat

        CreateMap<Chat, ChatDto>().ReverseMap();

        #endregion
    }
}
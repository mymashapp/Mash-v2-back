using Aimo.Core.Infrastructure;
using Aimo.Domain.Chats;
using Aimo.Domain.Users;
using Aimo.Domain.Users.Entities;
using AutoMapper;

namespace Aimo.Application.Chats;

internal class ChatMessageMapperProfile : Profile, IOrderedMapperProfile
{
    public int Order => 1;

    public ChatMessageMapperProfile()
    {
        CreateMap<ChatMessage, ChatMessageDto>()
            .ForMember(d => d.UserName, e => e.MapFrom(x => x.User.Name))
            .ForMember(d => d.CardId, e => e.MapFrom(x => x.Chat.CardId))
            //.ForMember(d => d.CardName, e => e.MapFrom(x => x..CardId))
            .ReverseMap();


        CreateMap<User, ChatMemberDto>()
            //.ForMember(d => d.PictureUrl, e => e.MapFrom(x => x.Pictures.Select(x => x.Url)))
            .ForMember(d => d.Id, e => e.MapFrom(x => x.Id));
        
        CreateMap<ChatMessage, IdNameDto>();
       
        CreateMap<PrivateMessageDto, ChatMessageDto>();
    }
}
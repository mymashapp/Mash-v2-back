using Aimo.Core.Infrastructure;
using Aimo.Domain.Chats;
using AutoMapper;

namespace Aimo.Application.Chats;

internal class ChatMessageMapperProfile : Profile, IOrderedMapperProfile
{
    public int Order => 1;

    public ChatMessageMapperProfile()
    {
        CreateMap<ChatMessage, ChatMessageDto>()
            .ForMember(d => d.UserName, e => e.MapFrom(x => x.User.Name))
            .ReverseMap();
    }
}
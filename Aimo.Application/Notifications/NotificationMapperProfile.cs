using Aimo.Core.Infrastructure;
using Aimo.Domain.Notifications;
using AutoMapper;

namespace Aimo.Data.Infrastructure.Yelp;

internal class NotificationMapperProfile : Profile, IOrderedMapperProfile
{
    public int Order => 1;

    public NotificationMapperProfile()
    {
        CreateMap<Notification, NotificationDto>().ReverseMap();
    }
}
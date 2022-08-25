using Aimo.Data.Infrastructure;
using Aimo.Domain.Notifications;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aimo.Data.Notifications;

public partial class NotificationTableMap : EntityTableMap<Notification>
{
    public override void Map(EntityTypeBuilder<Notification> builder)
    {
           
    }
}
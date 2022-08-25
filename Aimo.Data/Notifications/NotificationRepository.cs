using Aimo.Data.Infrastructure;
using Aimo.Domain.Data;
using Aimo.Domain.Notifications;

namespace Aimo.Data.Notifications
{
    internal partial class NotificationRepository : EfRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(IDataContext context) : base(context)
        {
        }
    }
    public partial interface INotificationRepository : IRepository<Notification>
    {
    }
}

namespace Aimo.Domain.Notifications;

public partial class Notification : Entity
{
    public int UserId { get; set; }
    public int NotificationTypeId { get; set; }
    public string Message { get; set; }
}
#nullable disable
namespace Aimo.Domain.Notifications;

public partial class NotificationDto : Dto
{
	public int UserId { get; set; }
	public int NotificationTypeId { get; set; }
	public string Message { get; set; }

}
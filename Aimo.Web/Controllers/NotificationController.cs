using Microsoft.AspNetCore.Mvc;
using Aimo.Application.Notifications;

namespace Aimo.Web.Controllers;

public class NotificationController : ApiBaseController
{
    #region ctor
    private readonly INotificationService _notificationService;

    public NotificationController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }
    #endregion
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Result(await _notificationService.Find());
    }
    
    
    [HttpPost]
    public async Task<IActionResult> DeleteNotification(int notificationId)
    {
        return Result(await _notificationService.Delete(notificationId));
    }
}
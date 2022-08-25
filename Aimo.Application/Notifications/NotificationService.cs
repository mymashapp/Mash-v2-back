using Aimo.Data.Notifications;
using Aimo.Domain.Infrastructure;
using Aimo.Domain.Notifications;
using Aimo.Domain.Users;

namespace Aimo.Application.Notifications;

internal partial class NotificationService : INotificationService
{
    #region ctor
    private readonly INotificationRepository _notificationRepository;
    private readonly IUserContext _userContext;

    public  NotificationService(
        INotificationRepository notificationRepository,IUserContext userContext
    )
    {
        _notificationRepository = notificationRepository;
        _userContext = userContext;
    }
    #endregion
    
    #region methods
    public async ResultTask Find()
    {
        var currentUserId = (await _userContext.GetCurrentUserAsync())?.Id;
        return Result.Create((await _notificationRepository.FindAsync(x=>x.UserId==currentUserId)).Map<NotificationDto[]>()).Success();
    }
  
    #endregion
    public async ResultTask Delete(int id)
    {
        var notification = await _notificationRepository.FirstOrDefaultAsync(x => x.Id == id);
        if (notification is null) return Result.Create().Failure(ResultMessage.NotFound);
        
        _notificationRepository.Remove(notification);
        await _notificationRepository.CommitAsync();
        return Result.Create().Success();

    }
}

public partial interface INotificationService
{
    ResultTask Find();
    ResultTask Delete(int notificationId);

}
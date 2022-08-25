using Aimo.Application.Infrastructure;
using Aimo.Data.Cards;
using Aimo.Data.Notifications;
using Aimo.Data.SwipeHistories;
using Aimo.Domain.Data;
using Aimo.Domain.Notifications;
using Aimo.Domain.ScheduleTasks;
using Aimo.Domain.Users;

namespace Aimo.Application.Cards;

internal partial class CardExpireNotificationBackgroundTask : BackgroundTask
{
    private readonly INotificationRepository _notificationRepository;
    private readonly ICardRepository _cardRepository;

    public CardExpireNotificationBackgroundTask(IRepository<ScheduleTask> repository,
        INotificationRepository notificationRepository,
        ICardRepository cardRepository) : base(repository)
    {
        _notificationRepository = notificationRepository;
        _cardRepository = cardRepository;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await NotificationExpireCardAsync();
    }

    private async ResultTask NotificationExpireCardAsync()
    {
        var result = Result.Create(false);
        try
        {
            
            var cards = await _cardRepository.GetUserIdForExpireCard();
            // .FindAsync(x => x.DateUtc > DateTime.Now.AddHours(-48) &&
            //               x.DateUtc <= DateTime.Now);

            if (!cards.Any())
                return result.Failure(ResultMessage.NotFound);

            var notification = cards.Select(x => new Notification
            {
                NotificationTypeId = (int)NotificationType.cardExpire,
                Message = $"Your event {x.Name} has expired.",
                UserId = x.CreatedBy
            });
            await _notificationRepository.AddBulkAsync(notification);
            _cardRepository.RemoveBulk(cards);
            var affected = await _notificationRepository.CommitAsync();
            return result.SetData(affected > 0, affected).Success();
        }
        catch (Exception e)
        {
            return result.Exception(e);
        }
    }

    protected override ScheduleTask DefaultInstance => new()
    {
        SystemName = SystemName,
        SystemType = SystemType,
        IsActive = true,
        IntervalInSeconds = TimeSpan.FromHours(48).TotalSeconds
    };
}
using Aimo.Data.SwipeHistories;
using Aimo.Domain.Data;
using Aimo.Domain.ScheduleTasks;
using Aimo.Domain.Users;

namespace Aimo.Application.SwipeHistories;

internal partial class DeleteSwipeHistoryBackgroundTask : BackgroundTask
{
    private readonly ISwipeHistoryRepository _swipeHistoryRepository;

    public DeleteSwipeHistoryBackgroundTask(IRepository<ScheduleTask> repository,
        ISwipeHistoryRepository swipeHistoryRepository) : base(repository)
    {
        _swipeHistoryRepository = swipeHistoryRepository;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await DeleteSwipesAsync(SwipeType.Left);
    }

    private async ResultTask DeleteSwipesAsync(SwipeType swipeType)
    {
        var result = Result.Create(false);
        try
        {
            var entities = await _swipeHistoryRepository.FindAsync(x => x.SwipeType == swipeType);

            if (!entities.IsNullOrEmpty())
                return result.Failure(ResultMessage.NotFound);

            _swipeHistoryRepository.RemoveBulk(entities);
            var affected = await _swipeHistoryRepository.CommitAsync();
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
        IntervalInSeconds = TimeSpan.FromHours(24).TotalSeconds
    };
}
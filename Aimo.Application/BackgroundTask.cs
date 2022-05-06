using Aimo.Core.Infrastructure;
using Aimo.Domain.Data;
using Aimo.Domain.ScheduleTasks;
using Microsoft.Extensions.Hosting;

namespace Aimo.Application;

internal abstract partial class BackgroundTask : /*BackgroundService*/ IBackgroundTask, IHostedService
{
    protected readonly IRepository<ScheduleTask> Repository;
    public string SystemType => GetType()?.FullName!;
    public string SystemName => GetType()?.Name!;

    protected abstract Task ExecuteAsync(CancellationToken stoppingToken);

    protected BackgroundTask(IRepository<ScheduleTask> repository)
    {
        Repository = repository;
    }

    protected abstract ScheduleTask DefaultInstance { get; }

    protected virtual async Task<ScheduleTask> Install()
    {
        var bgTask = await Repository
            .FirstOrDefaultAsync(x => x.SystemName == SystemName);

        if (bgTask is not null)
            return bgTask;

        bgTask = DefaultInstance;
        await Repository.AddAsync(bgTask);
        await Repository.CommitAsync();
        return bgTask;
    }

    public /*override*/ async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            var currentTask = await Install();

            if (!currentTask.IsActive)
            {
                await StopAsync(cancellationToken);
                return;
            }

            while (!cancellationToken.IsCancellationRequested)
            {
                if (IsTaskAlreadyRunning(currentTask))
                    continue;

                var remainingTime = RemainingTimeToExecute(currentTask);
                await Task.Delay(remainingTime, cancellationToken);

                currentTask.StartedOnUtc = DateTime.UtcNow;

                Repository.Update(currentTask);
                await Repository.CommitAsync(ct: cancellationToken);

                await PreExecuteAsync();
                await ExecuteAsync(cancellationToken);
                await PostExecuteAsync();

                currentTask.SucceededOnUtc = DateTime.UtcNow;
                Repository.Update(currentTask);
                await Repository.CommitAsync(ct: cancellationToken);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private static TimeSpan RemainingTimeToExecute(ScheduleTask currentTask)
    {
        var remaining = TimeSpan.FromSeconds(currentTask.IntervalInSeconds) -
                        (DateTime.UtcNow - currentTask.SucceededOnUtc ?? TimeSpan.FromSeconds(0));

        remaining = remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
        return remaining;
    }

    protected virtual bool IsTaskAlreadyRunning(ScheduleTask scheduleTask)
    {
        //task run for the first time
        if (!scheduleTask.StartedOnUtc.HasValue && !scheduleTask.SucceededOnUtc.HasValue)
            return false;

        var lastStartUtc = scheduleTask.SucceededOnUtc ?? DateTime.UtcNow;

        //task already finished
        if (scheduleTask.SucceededOnUtc.HasValue && lastStartUtc < scheduleTask.SucceededOnUtc)
            return false;

        //task wasn't finished last time
        if (lastStartUtc.AddSeconds(scheduleTask.IntervalInSeconds) <= DateTime.UtcNow)
            return false;

        return true;
    }

    protected virtual Task PreExecuteAsync()
    {
        return Task.CompletedTask;
    }

    protected virtual Task PostExecuteAsync()
    {
        return Task.CompletedTask;
    }

    protected virtual Task StopTaskAsync(CancellationToken ct)
    {
        return Task.CompletedTask;
    }

    public /*override*/ async Task StopAsync(CancellationToken cancellationToken)
    {
        await StopTaskAsync(cancellationToken);
    }
}
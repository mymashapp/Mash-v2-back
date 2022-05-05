#nullable disable
namespace Aimo.Domain.ScheduleTasks;

public class ScheduleTask : Entity , IActiveInactiveSupport
{
    public string SystemName { get; set; }
    public string SystemType { get; set; }
    public DateTime? StartedOnUtc { get; set; }
    public DateTime? SucceededOnUtc { get; set; }
    public double IntervalInSeconds { get; set; }
    public bool IsActive { get; set; }
}
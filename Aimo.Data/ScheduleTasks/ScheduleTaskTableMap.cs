using Aimo.Data.Infrastructure;
using Aimo.Domain.ScheduleTasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aimo.Data.ScheduleTasks;

public partial class ScheduleTaskTableMap : EntityTableMap<ScheduleTask>
{
    public override void Map(EntityTypeBuilder<ScheduleTask> builder)
    {
        builder.HasIndex(t => t.SystemName).IsUnique();
    }
}
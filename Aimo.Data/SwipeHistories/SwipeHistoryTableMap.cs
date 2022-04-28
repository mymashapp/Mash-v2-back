using Aimo.Data.Infrastructure;
using Aimo.Domain.SwipeHistories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aimo.Data.SwipeHistories;

public partial class SwipeHistoryTableMap : EntityTableMap<SwipeHistory>
{
    public override void Map(EntityTypeBuilder<SwipeHistory> builder)
    {
        builder.Property(t=>t.SeenAtUtc).HasDefaultValueSql("GETUTCDATE()");
        builder.HasOne(t => t.Card).WithMany().HasForeignKey(t => t.CardId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(t => t.User).WithMany().HasForeignKey(t => t.UserId).OnDelete(DeleteBehavior.Cascade);

    }
}
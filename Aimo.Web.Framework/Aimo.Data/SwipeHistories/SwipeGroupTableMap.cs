using Aimo.Data.Infrastructure;
using Aimo.Domain.SwipeHistories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aimo.Data.SwipeHistories;

public partial class SwipeGroupTableMap : EntityTableMap<SwipeGroup>
{
    public override void Map(EntityTypeBuilder<SwipeGroup> builder)
    {
        builder.Property(t => t.UserId).IsRequired();
        builder.Property(t => t.CardId).IsRequired();
        builder.Property(t => t.AgeFrom).IsRequired();
        builder.Property(t => t.AgeTo).IsRequired();
        builder.Property(t => t.Gender).IsRequired();

        builder.HasMany(t => t.Interests).WithMany(t => t.SwipeGroups).UsingEntity<SwipeGroupInterest>(
            l => l.HasOne(t => t.Interest).WithMany().HasForeignKey(t => t.InterestId).OnDelete(DeleteBehavior.Cascade),
            r => r.HasOne(t => t.SwipeGroup).WithMany().HasForeignKey(t => t.SwipeGroupId)
                .OnDelete(DeleteBehavior.Cascade)
        ).ToTable(nameof(SwipeGroupInterest));
    }
}
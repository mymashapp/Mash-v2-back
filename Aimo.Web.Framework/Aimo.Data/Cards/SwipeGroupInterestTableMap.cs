using Aimo.Data.Infrastructure;
using Aimo.Domain.Cards;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aimo.Data.Cards;

public partial class SwipeGroupInterestTableMap : EntityTableMap<SwipeGroupInterest>
{
    public override void Map(EntityTypeBuilder<SwipeGroupInterest> builder)
    {
        builder.HasOne(t => t.Interest).WithMany().HasForeignKey(t => t.InterestId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(t => t.SwipeGroup).WithMany().HasForeignKey(t => t.SwipeGroupId).OnDelete(DeleteBehavior.Cascade);
    }
}
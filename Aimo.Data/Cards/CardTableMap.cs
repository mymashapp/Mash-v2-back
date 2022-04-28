using Aimo.Data.Infrastructure;
using Aimo.Domain.Cards;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aimo.Data.Cards;

public partial class CardTableMap : EntityTableMap<Card>
{
    public override void Map(EntityTypeBuilder<Card> builder)
    {
        builder.Property(t => t.Name).IsRequired();
        builder.Property(t => t.Longitude).IsRequired();
        builder.Property(t => t.Latitude).IsRequired();
        builder.Property(t => t.ZipCode).IsRequired();
        builder.Property(t => t.CategoryId).IsRequired(false);

        //builder.HasOne(t => t.Category).WithMany().HasForeignKey(t => t.CategoryId).OnDelete(DeleteBehavior.Restrict).IsRequired(false);

    }
}
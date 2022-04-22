using Aimo.Data.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aimo.Data.Cards;

public partial class CardTableMap : EntityTableMap<Domain.Cards.Card>
{
    public override void Map(EntityTypeBuilder<Domain.Cards.Card> builder)
    {
        builder.Property(t => t.Name).IsRequired();
        builder.Property(t => t.Longitude).IsRequired();
        builder.Property(t => t.Latitude).IsRequired();
        builder.Property(t => t.ZipCode).IsRequired();

        builder.HasOne(t => t.Category).WithMany().HasForeignKey(t => t.CategoryId).OnDelete(DeleteBehavior.Restrict);

    }
}
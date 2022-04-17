using Aimo.Data.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aimo.Data.Card;

public partial class CardTableMap : EntityTableMap<Domain.Card.Card>
{
    public override void Map(EntityTypeBuilder<Domain.Card.Card> builder)
    {
        builder.Property(t => t.EventName).IsRequired();
        builder.Property(t => t.DateUTC).IsRequired();
        builder.Property(t => t.longitude).IsRequired();
        builder.Property(t => t.latitude).IsRequired();
        builder.Property(t => t.Address).IsRequired();
        builder.Property(t => t.Zip).IsRequired();
        builder.Property(t => t.CardType).IsRequired();
    }
}
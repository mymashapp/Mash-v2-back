using Aimo.Data.Infrastructure;
using Aimo.Domain.Cards;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aimo.Data.Cards;

public partial class CardPictureTableMap : EntityTableMap<CardPicture>
{
    public override void Map(EntityTypeBuilder<CardPicture> builder)
    {
        builder.Property(t => t.CardId).IsRequired();
        builder.Property(t => t.PictureUrl).IsRequired();
    }
}
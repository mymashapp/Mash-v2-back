using Aimo.Data.Infrastructure;
using Aimo.Domain.Categories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aimo.Data.Categories;

public partial class SubCategoryTableMap : EntityTableMap<SubCategory>
{
    public override void Map(EntityTypeBuilder<SubCategory> builder)
    {
        builder.Property(t => t.Title).IsRequired();
        builder.Property(t => t.Alias).IsRequired();
        builder.Property(t => t.DisplayOrder).IsRequired();

        builder.HasOne(t => t.Category).WithMany(t => t.SubCategories).HasForeignKey(t => t.CategoryId).OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Cards).WithMany(x => x.SubCategories)
            .UsingEntity<CardSubCategory>(
                ur => ur.HasOne(t => t.Card)
                    .WithMany()
                    .HasForeignKey(t => t.CardId)
                    .OnDelete(DeleteBehavior.Cascade),
                ur => ur.HasOne(t => t.SubCategory)
                    .WithMany()
                    .HasForeignKey(t => t.SubCategoryId)
                    .OnDelete(DeleteBehavior.Cascade)
            ).Ignore(t => t.Id);
    }
}

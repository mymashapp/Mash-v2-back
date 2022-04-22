using Aimo.Data.Infrastructure;
using Aimo.Domain.Categories;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aimo.Data.Categories;

public partial class CategoryTableMap : EntityTableMap<Category>
{
    public override void Map(EntityTypeBuilder<Category> builder)
    {
        builder.Property(t => t.Name).IsRequired();
        builder.Property(t => t.DisplayOrder).IsRequired();
    }
}
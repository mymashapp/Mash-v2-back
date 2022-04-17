using Aimo.Data.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aimo.Data.Category;

public partial class CategoryTableMap : EntityTableMap<Domain.Category.Category>
{
    public override void Map(EntityTypeBuilder<Domain.Category.Category> builder)
    {
        builder.Property(t => t.Name).IsRequired();
        builder.Property(t => t.DisplayOrder).IsRequired();
    }
}
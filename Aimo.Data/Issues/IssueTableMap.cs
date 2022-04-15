using Aimo.Data.Infrastructure;
using Aimo.Domain.Issues;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aimo.Data.Issues;

public partial class IssueTableMap : EntityTableMap<Issue>
{
    public override void Map(EntityTypeBuilder<Issue> builder)
    {
        builder.Property(t => t.Title).IsRequired().HasMaxLength(250);
        builder.Property(t => t.Description).IsRequired();
        builder.Property(t => t.ExceptedBehaviour).IsRequired();
    }
}
using Aimo.Data.Infrastructure;
using Aimo.Domain.Users;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aimo.Data.Users
{
    public partial class UserPreferencesTableMap : EntityTableMap<UserPreference>
    {
        public override void Map(EntityTypeBuilder<UserPreference> builder)
        {
            builder.Property(t => t.UserId).IsRequired();
        }
    }
}

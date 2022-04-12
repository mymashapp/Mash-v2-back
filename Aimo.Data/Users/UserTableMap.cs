using Aimo.Data.Infrastructure;
using Aimo.Domain.Users;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aimo.Data.Users
{
    public partial class UserTableMap : EntityTableMap<User>
    {
        public override void Map(EntityTypeBuilder<User> builder)
        {
            builder.Property(t => t.Name).IsRequired().HasMaxLength(250);
            builder.Property(t => t.Email).IsRequired();
            builder.Property(t => t.DateOfBirth).IsRequired();
            builder.Property(t => t.Gender).IsRequired();
        }
    }
}

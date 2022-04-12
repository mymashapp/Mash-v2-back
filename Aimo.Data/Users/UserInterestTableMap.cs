using Aimo.Data.Infrastructure;
using Aimo.Domain.Users;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aimo.Data.Users
{
    public partial class UserInterestTableMap : EntityTableMap<UserInterest>
    {
        public override void Map(EntityTypeBuilder<UserInterest> builder)
        {
            builder.Property(t => t.UserId).IsRequired();
            builder.Property(t => t.InterestId).IsRequired();
        }
    }
}
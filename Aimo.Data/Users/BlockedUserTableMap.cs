using Aimo.Data.Infrastructure;
using Aimo.Domain.Users;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aimo.Data.Users;

public partial class BlockedUserTableMap : EntityTableMap<BlockedUser>
{
    public override void Map(EntityTypeBuilder<BlockedUser> builder)
    {
           
    }
}
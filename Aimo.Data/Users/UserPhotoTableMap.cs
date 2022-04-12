using Aimo.Data.Infrastructure;
using Aimo.Domain.Users;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aimo.Data.Users
{
    public partial class UserPhotoTableMap:EntityTableMap<UserPhoto>
    {
        public override void Map(EntityTypeBuilder<UserPhoto> builder)
        {
            builder.Property(t => t.UserId).IsRequired();
            builder.Property(t => t.PictureId).IsRequired();
        }
    }
}

using Aimo.Data.Infrastructure;
using Aimo.Domain.Chats;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aimo.Data.Chats;

public partial class UserDeletedChatTableMap : EntityTableMap<UserDeletedChat>
{
    public override void Map(EntityTypeBuilder<UserDeletedChat> builder)
    {
        builder.HasOne(t => t.Chat).WithMany().HasForeignKey(t => t.ChatId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(t => t.User).WithMany().HasForeignKey(t => t.UserId).OnDelete(DeleteBehavior.Cascade);

    }
}
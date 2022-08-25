using Aimo.Data.Infrastructure;
using Aimo.Domain.Chats;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aimo.Data.Chats;

public partial class ChatTableMap : EntityTableMap<Chat>
{
    public override void Map(EntityTypeBuilder<Chat> builder)
    {
        builder.Property(t => t.CardId).IsRequired();
        builder.Property(t => t.GroupType).IsRequired();
        builder.Property(t => t.ChatType).IsRequired();

        builder.HasMany(t => t.Users).WithMany(t => t.Chats)
            .UsingEntity<ChatUser>
            (l => l.HasOne(t => t.User).WithMany().HasForeignKey(t => t.UserId).OnDelete(DeleteBehavior.Restrict),
                r => r.HasOne(t => t.Chat).WithMany().HasForeignKey(t => t.ChatId).OnDelete(DeleteBehavior.Restrict)
            )
            .ToTable(nameof(ChatUser));
            //.Property(t=>t.JoinedOnUtc).HasDefaultValueSql("GETUTCDATE()");
    }
}
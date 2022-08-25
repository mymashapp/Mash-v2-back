using Aimo.Data.Infrastructure;
using Aimo.Domain.Chats;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aimo.Data.Chats;

public partial class ChatMessageTableMap : EntityTableMap<ChatMessage>
{
    public override void Map(EntityTypeBuilder<ChatMessage> builder)
    {
        builder.Property(t=>t.SendOnUtc).HasDefaultValueSql("GETUTCDATE()");
        builder.Property(x => x.Text).IsRequired();
        builder.HasOne(t => t.Chat).WithMany().HasForeignKey(t => t.ChatId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(t => t.User).WithMany().HasForeignKey(t => t.SenderUserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(t => t.SendOnUtc);
    }
}
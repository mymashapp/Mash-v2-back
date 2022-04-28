namespace Aimo.Data.Chats;

/*
public partial class ChatUserTableMap : EntityTableMap<ChatUser>
{
    public override void Map(EntityTypeBuilder<ChatUser> builder)
    {
        builder.HasOne(t => t.Chat).WithMany().HasForeignKey(t => t.ChatId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(t => t.User).WithMany().HasForeignKey(t => t.UserId).OnDelete(DeleteBehavior.Cascade);

    }
}*/
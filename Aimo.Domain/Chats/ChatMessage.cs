using Aimo.Domain.Users.Entities;

namespace Aimo.Domain.Chats;

public partial class ChatMessage : Entity,ISoftDeleteSupport
{
    public int ChatId { get; set; }
    public int SenderUserId { get; set; }
    public string Text { get; set; }
    public DateTime SendOnUtc { get; set; }
    
    public bool IsDeleted { get; set; }
    public User? User { get; set; }
    public Chat Chat { get; set; }
}
using Aimo.Domain.Users.Entities;

namespace Aimo.Domain.Chats;

public partial class ChatUser : Entity
{
    public int UserId { get; set; }
    public int ChatId { get; set; }
    
    public DateTime JoinOnUtc { get; set; }
   // public DateTime JoinedOnUtc { get; set; }
    
    public User User { get; set; }
    public Chat Chat { get; set; }
}
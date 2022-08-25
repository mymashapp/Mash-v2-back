using Aimo.Domain.Users.Entities;

namespace Aimo.Domain.Chats;

public partial class UserDeletedChat : Entity
{
    public int UserId { get; set; }
    public int ChatId { get; set; }
    public User User { get; set; }
    public Chat Chat { get; set; }
}
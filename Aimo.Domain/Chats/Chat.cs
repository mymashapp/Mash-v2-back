using Aimo.Domain.Cards;
using Aimo.Domain.Users;
using Aimo.Domain.Users.Entities;

namespace Aimo.Domain.Chats;

public partial class Chat : Entity
{
    public int CardId { get; set; }
    public GroupType GroupType { get; set; }
    public Card Card { get; set; } 
    public List<User> Users { get; init; } = new();
}


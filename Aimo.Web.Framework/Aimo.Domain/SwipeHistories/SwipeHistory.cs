using Aimo.Domain.Cards;
using Aimo.Domain.Users;
using Aimo.Domain.Users.Entities;

namespace Aimo.Domain.SwipeHistories;

public partial class SwipeHistory : Entity
{
    public int CardId { get; set; }
    public int UserId { get; set; }
    public DateTime SeenAtUtc { get; set; }

    public SwipeType SwipeType { get; set; }
    public Card Card { get; set; } 
    public User User { get; set; }
}
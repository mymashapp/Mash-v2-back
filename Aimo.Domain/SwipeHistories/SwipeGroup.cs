using Aimo.Domain.Cards;
using Aimo.Domain.Interests;
using Aimo.Domain.Users;
using Aimo.Domain.Users.Entities;

namespace Aimo.Domain.SwipeHistories;

public partial class SwipeGroup : Entity
{
    public int UserId { get; set; }
    public int CardId { get; set; }
    public int AgeTo { get; set; }
    public int AgeFrom { get; set; }
    public Gender Gender { get; set; }
    public GroupType GroupType { get; set; } = new();
    public User User { get; set; } 
    public Card Card { get; set; } 
    public int CurrentUserAge { get; set; } 
    

    public List<Interest> Interests { get; set; }
}
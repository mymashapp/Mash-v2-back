using Aimo.Domain.Interests;
using Aimo.Domain.Users;

namespace Aimo.Domain.Cards;

public partial class SwipeGroup : Entity
{
    public int UserId { get; set; }
    public int AgeTo { get; set; }
    public int AgeFrom { get; set; }
    public Gender Gender { get; set; }
    public GroupType GroupType { get; set; }

    public List<Interest> Interests { get; init; } = new();
}
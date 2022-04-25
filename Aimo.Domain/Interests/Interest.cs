using Aimo.Domain.Cards;
using Aimo.Domain.Users.Entities;

namespace Aimo.Domain.Interests;

public partial class Interest : Entity
{
    public string Name { get; set; }
    public int DisplayOrder { get; set; }
    public List<User> Users { get; init; } = new();
    public List<SwipeGroup> SwipeGroups { get; init; } = new();
}
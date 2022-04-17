using Aimo.Domain.Users.Entities;

namespace Aimo.Domain.Interests;

public partial class InterestDto : Dto
{
    public string Name { get; set; }
    public int DisplayOrder { get; set; }
   // public List<User> Users { get; init; } = new();
}
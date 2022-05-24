using Aimo.Domain.Users;

namespace Aimo.Domain.SwipeHistories;

public partial class SwipeGroupDto : Dto
{
    public int UserId { get; set; }
    public int CardId { get; set; }
    public int AgeTo { get; set; }
    public int AgeFrom { get; set; }
    public int CurrentUserAge { get; set; } 

    public Gender Gender { get; set; }
    public GroupType GroupType { get; set; }
    
    public List<UserInterestDto> UserInterests { get; set; } = new();


}
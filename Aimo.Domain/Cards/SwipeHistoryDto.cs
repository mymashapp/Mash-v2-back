using Aimo.Domain.Users;

namespace Aimo.Domain.Cards;

public partial class SwipeHistoryDto : Dto
{
    public int CardId { get; set; }
    public int UserId { get; set; }
    public DateTime SeenAtUtc { get; set; }

    public SwipeType SwipeType { get; set; }
    
}
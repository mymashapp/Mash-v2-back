using Aimo.Domain.Interests;

namespace Aimo.Domain.SwipeHistories;

public partial class SwipeGroupInterest 
{
    public int SwipeGroupId { get; set; }

    public int InterestId { get; set; }

    public virtual SwipeGroup SwipeGroup { get; set; }

    public virtual Interest Interest { get; set; }
}
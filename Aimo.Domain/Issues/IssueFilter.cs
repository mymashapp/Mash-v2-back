namespace Aimo.Domain.Issues;

public partial class IssueFilter : Filter
{
    public IssueFilter()
    {
        SortColumn = nameof(Id);
        SortDirection = SortDirection.Desc;
    }

    public int?[] ProjectIds { get; set; }
    public int?[] StatusIds { get; set; }
    public int?[] SeverityIds { get; set; }
    public int?[] CategoryIds { get; set; }
    public int?[] PriorityIds { get; set; }
    public DateTime? FromUtc { get; set; }
    public DateTime? ToUtc { get; set; }
}
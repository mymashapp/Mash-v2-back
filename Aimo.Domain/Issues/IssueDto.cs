#nullable disable
namespace Aimo.Domain.Issues;

public partial class IssueCountDto
{
    public int Count { get; set; }
    public string Status { get; set; }
}

public partial class IssueDto : Dto
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string ExceptedBehaviour { get; set; }

    public string Project { get; set; }
    public string Status { get; set; }
    public string Severity { get; set; }
    public string Category { get; set; }
    public string Priority { get; set; }

    public int ProjectId { get; set; }
    public int StatusId { get; set; }
    public int SeverityId { get; set; }
    public int PriorityId { get; set; }
}
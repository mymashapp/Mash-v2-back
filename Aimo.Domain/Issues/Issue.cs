#nullable disable
namespace Aimo.Domain.Issues;

public partial class Issue : AuditableEntity
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string ExceptedBehaviour { get; set; }

    public int ProjectId { get; set; }
    public int StatusId { get; set; }
    public int SeverityId { get; set; }
    public int CategoryId { get; set; }
    public int PriorityId { get; set; }

    public Project Project { get; set; }
    public IssueStatus Status { get; set; }
    public IssueSeverity Severity { get; set; }
    public IssuePriority Priority { get; set; }
    public IssueCategory Category { get; set; }
}
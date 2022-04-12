using Aimo.Data;
using Aimo.Domain.Issues;

namespace Aimo.Application.Issues;

internal partial class IssueQuery : FilterQuery<Issue, IssueFilter>
{
    public IssueQuery(IQueryable<Issue> query, IssueFilter filter) : base(query, filter)
    {
    }

    private void ProjectIds() => Query = Query.Where(x => Filter.ProjectIds.Contains(x.ProjectId));

    private void StatusIds() => Query = Query.Where(x => Filter.StatusIds.Contains(x.StatusId));

    private void SeverityIds() => Query = Query.Where(x => Filter.SeverityIds.Contains(x.SeverityId));

    private void CategoryIds() => Query = Query.Where(x => Filter.CategoryIds.Contains(x.CategoryId));

    private void PriorityIds() => Query = Query.Where(x => Filter.PriorityIds.Contains(x.PriorityId));

    private void FromUtc() => Query = Query.Where(x => x.CreatedAtUtc >= Filter.FromUtc);

    private void ToUtc() => Query = Query.Where(x => x.CreatedAtUtc <= Filter.ToUtc);
}
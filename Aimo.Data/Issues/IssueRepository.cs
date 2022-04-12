using Aimo.Data.Infrastructure;
using Aimo.Domain;
using Aimo.Domain.Data;
using Aimo.Domain.Issues;
using Microsoft.EntityFrameworkCore;

namespace Aimo.Data.Issues;

internal partial class IssueRepository : EfRepository<Issue>, IIssueRepository
{
    public IssueRepository(IDataContext context) : base(context)
    {
    }

    public async Task<IssueCountDto[]> GetIssueCountByStatusAsync()
    {
        var issues = AsNoTracking;
        var statuses = AsNoTracking<IssueStatus>();

        var query = from i in issues
            from s in statuses.Where(x => x.Id == i.StatusId)
            group i by new { s.Name, i.StatusId }
            into g
            select new IssueCountDto
            {
                Status = g.Key.Name,
                Count = g.Count()
            };
        return await query.ToArrayAsync();
    }
}

public partial interface IIssueRepository : IRepository<Issue>
{
    Task<IssueCountDto[]> GetIssueCountByStatusAsync();
}
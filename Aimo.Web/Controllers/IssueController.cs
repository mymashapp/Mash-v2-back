using Aimo.Application.Issues;
using Aimo.Core;
using Aimo.Domain.Issues;
using Microsoft.AspNetCore.Mvc;

namespace Aimo.Web.Controllers;

public class IssueController : ApiBaseController
{
    private readonly IIssueService _issueService;


    public IssueController(IIssueService issueService)
    {
        _issueService = issueService;
    }

    [HttpPost]
    public async Task<ListResult<IssueDto>> Post(IssueFilter filter)
    {
        return await _issueService.Find(filter);
    }

    [HttpGet("GetCount")]
    public async Task<ListResult<IssueCountDto>> GetCount()
    {
        return await _issueService.GetCountByStatus();
    }
}
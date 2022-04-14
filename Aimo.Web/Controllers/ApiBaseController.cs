using Aimo.Core;
using Aimo.Web.Framework;
using Microsoft.AspNetCore.Mvc;

namespace Aimo.Web.Controllers;

[ApiController, AppAuthorize, Route("api/[controller]/[action]")]
public class ApiBaseController : ControllerBase
{
    /*protected async Task<IActionResult> ResultAsync(Task<Result> resultTask)
    {
        var result = await resultTask;
        return result.ThrowIfNull().IsSucceeded ? Ok(result) : BadRequest(result);
    }*/

    protected IActionResult Result(Result result)
    {
        return result.ThrowIfNull().IsSucceeded
            ? Ok(result)
            : result.Message == ResultMessage.NotFound
                ? NotFound(result)
                : BadRequest(result);
    }
}
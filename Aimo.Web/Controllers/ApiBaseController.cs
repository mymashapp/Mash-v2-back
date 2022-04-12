using Aimo.Web.Framework;
using Microsoft.AspNetCore.Mvc;

namespace Aimo.Web.Controllers;

[ApiController, AppAuthorize,Route("api/[controller]")]
public class ApiBaseController : ControllerBase
{
}
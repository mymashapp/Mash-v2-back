using Aimo.Application.Users;
using Aimo.Domain.Users;
using Microsoft.AspNetCore.Mvc;

namespace Aimo.Web.Controllers;

[ApiController]
public class UserController : ApiBaseController
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPut]
    public async Task<IActionResult> Update(UserSaveDto viewDto)
    {
        return Result(await _userService.Update(viewDto));
    }

    [HttpGet]
    public async Task<IActionResult> GetUserById(int id)
    {
        return Result(await _userService.GetById(id));
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllInterests()
    {
        return Ok(await _userService.GetAllInterests());
    }

    [HttpGet]
    public async Task<IActionResult> GetUserByUid(string uid)
    {
        return Result(await _userService.GetOrCreateUserByUidAsync(uid));
    }
}
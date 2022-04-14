using Aimo.Application.Users;
using Aimo.Core;
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
    public async Task<IActionResult> Update(UserDto dto)
    {
        return Result(await _userService.Update(dto));
    }

    [HttpGet]
    public async Task<IActionResult> GetUserById(int id)
    {
        return Result(await _userService.GetById(id));
    }

    [HttpGet]
    public async Task<IActionResult> GetUserByUid(string uid)
    {
        return Result(await _userService.GetOrCreateUserByUidAsync(uid));
    }
}
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

    /*[HttpGet]
    public async Task<IActionResult> GetUserById(int id)
    {
        return Result(await _userService.GetById(id));
    }*/
    
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
    
    [HttpPut]
    public async Task<IActionResult> Update(UserDto dto)
    {
        return Result(await _userService.Update(dto));
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdatePictures(PictureDto[] dto)
    {
        return Result(await _userService.UpdatePictures(dto));
    }
}
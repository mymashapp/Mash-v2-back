using Aimo.Application.Users;
using Aimo.Domain.Users;
using Microsoft.AspNetCore.Mvc;

namespace Aimo.Web.Controllers;

[ApiController]
public class UserController : ApiBaseController
{
    private readonly IUserService _userService;
    private readonly IUserPicturesService _userPicturesService;

    public UserController(IUserService userService,IUserPicturesService userPicturesService)
    {
        _userService = userService;
        _userPicturesService = userPicturesService;
    }

    /*[HttpGet]
    public async Task<IActionResult> GetUserById(int id)
    {
        return Result(await _userService.GetByIdAsync(id));
    }*/
    
    /*
    [HttpGet]
    public async Task<IActionResult> GetAllInterests()
    {
        return Ok(await _userService.GetAllInterests());
    }
    */

    [HttpGet]
    public async Task<IActionResult> GetUserByUid(string uid)
    {
        return Result(await _userService.GetOrCreateUserByUidAsync(uid));
    }
    
    [HttpPut]
    public async Task<IActionResult> Update(UserDto dto)
    {
        return Result(await _userService.UpdateAsync(dto));
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdatePictures(UserPictureDto[] dto)
    {
        return Result(await _userService.UpdatePicturesAsync(dto));
    } 
    [HttpPost]
    public async Task<IActionResult> AddMediaPictures(UserPictureDto dto)
    {
        return Result(await _userPicturesService.AddUserMediaPicture(dto));
    }
    
    [HttpPost]
    public async Task<IActionResult> DeletePictures(params int[] ids)
    {
        return Result(await _userPicturesService.DeletePicturesAsync(ids));
    }
}
using Aimo.Application.FireBasePushNotifications;
using Aimo.Application.Users;
using Aimo.Domain.FireBasePushNotifications;
using Aimo.Domain.Users;
using Aimo.Domain.Users.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Aimo.Web.Controllers;

[ApiController]
public class UserController : ApiBaseController
{
    private readonly IUserService _userService;
    private readonly IUserPicturesService _userPicturesService;
    private readonly IUserLocationService _userLocationService;
    private readonly IFireBasePushNotificationService _fireBasePushNotificationService;

    public UserController(IUserService userService,IUserPicturesService userPicturesService,IUserLocationService userLocationService,
        IFireBasePushNotificationService fireBasePushNotificationService)
    {
        _userService = userService;
        _userPicturesService = userPicturesService;
        _userLocationService = userLocationService;
        _fireBasePushNotificationService = fireBasePushNotificationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetUserById(int id)
    {
        return Result(await _userService.GetByIdAsync(id));
    }

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
    
    [HttpPut]
    public async Task<IActionResult> AddUserLocation(UserLocationDto dto)
    {
        return Result(await _userLocationService.UpdateAsync(dto));
    }  
    [HttpPost]
    public async Task<IActionResult> GetUserIn10Mile(UserLocationDto dto)
    {
        dto.Distance = 10;
        return Result(await _userLocationService.GetUserInXMile(dto));
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

    [HttpPost]
    public async Task<IActionResult> DeleteUser(int id)
    {
        return Result(await _userService.DeleteAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> ToggleUserLocation(int id)
    {
        return Result(await _userService.ToggleUserLocationAsync(userId: id));
    }

    [HttpPost]
    public async Task<IActionResult> BlockUser(int blockUserId)
    {
        return Result(await _userService.BlockUserAsync(blockUserId: blockUserId));
    }

    [HttpPost]
    public async Task<IActionResult> UnBlockUser(int unBlockUserId)
    {
        return Result(await _userService.UnBlockUserAsync(unBlockUserId: unBlockUserId));
    }

    [HttpPost]
    public async Task<IActionResult> BlockedUsersForCurrentUser()
    {
        return Result(await _userService.GetAllBlockedUsersForCurrentUser());
    }

    [HttpPost]
    public async Task<IActionResult> SetUserFCMToken(string uid, string FCMtoken)
    {
        return Result(await _userService.SetUserFCMTokenAsync(uid,FCMtoken));
    }

    [HttpPost]
    public async Task<IActionResult> CreateNotification(FireBasePushNotificationDto dto)
    {
        var s = await _fireBasePushNotificationService.SendPushNotification(dto);
        return Result(s);
    }
}
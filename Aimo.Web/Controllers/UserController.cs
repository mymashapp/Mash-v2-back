using Aimo.Application.Users;
using Aimo.Core;
using Aimo.Domain.Users;
using Microsoft.AspNetCore.Mvc;

namespace Aimo.Web.Controllers
{
    public class UserController : ApiBaseController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<Result<UserDto>> SaveUserDetail(UserDto dto)
        {
            return await _userService.Update(dto);
        }

        public async Task<Result<UserDto>> GetUserById(int id)
        {
            return await _userService.GetById(id);
        }

        public async Task<Result<UserDto>> GetUserById(string uid)
        {
            return await _userService.GetOrCreateUserByUidAsync(uid);
        }
    }
}

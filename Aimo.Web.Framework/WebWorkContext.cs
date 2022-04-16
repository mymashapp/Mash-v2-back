using Aimo.Domain.Data;
using Aimo.Domain.Users.Entities;
using Aimo.Domain.WorkContext;
using Microsoft.AspNetCore.Http;

namespace Aimo.Web.Framework;

internal class WebWorkContext : IWorkContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRepository<User> _userRepository;
    private User? _cachedUser = null!;

    public WebWorkContext(IHttpContextAccessor httpContextAccessor, IRepository<User> userRepository)
    {
        _httpContextAccessor = httpContextAccessor;
        _userRepository = userRepository;
    }


    public async Task<User?> GetCurrentUserAsync(bool ensureNotNull = false)
    {
        if (_cachedUser is not null)
            return _cachedUser;

        var uuid = _httpContextAccessor.HttpContext?.Items[WebFrameworkDefaults.UserUniqueId];
        if (uuid is not null)
            _cachedUser = (await _userRepository.FindAsync(x => x.Uid == uuid.ToString())).FirstOrDefault();

        return _cachedUser;
    }


    public virtual async Task SetCurrentUserAsync(User? user = null)
    {
        _cachedUser = null!;

        if (user is not null && !user.IsDeleted && user.IsActive)
        {
            //cache the found user
            _cachedUser = user;
        }

        if (user is null || user.IsDeleted || !user.IsActive)
        {
            var uuid = _httpContextAccessor.HttpContext?.Items[WebFrameworkDefaults.UserUniqueId];
            if (uuid is not null)
                _cachedUser = (await _userRepository.FindAsync(x => x.Uid == uuid.ToString())).FirstOrDefault();
        }
    }
}
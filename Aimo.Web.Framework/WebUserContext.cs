using System.Security.Claims;
using Aimo.Domain.Data;
using Aimo.Domain.Users;
using Aimo.Domain.Users.Entities;
using Microsoft.AspNetCore.Http;

namespace Aimo.Web.Framework;

internal class WebUserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRepository<User> _userRepository;
    private User? _cachedUser = null!;

    public WebUserContext(IHttpContextAccessor httpContextAccessor, IRepository<User> userRepository)
    {
        _httpContextAccessor = httpContextAccessor;
        _userRepository = userRepository;
    }


    public virtual async Task<User?> GetCurrentUserAsync(bool ensureNotNull = false)
    {
        if (_cachedUser is not null)
            return _cachedUser;

        var uuid = _httpContextAccessor.HttpContext?.Items[WebFrameworkDefaults.UserUniqueId] ??
                   _httpContextAccessor.HttpContext?.User?.Identities?
                       .FirstOrDefault(x => x.AuthenticationType == "Firebase")?.Claims?
                       .FirstOrDefault(x => x.Type == nameof(_cachedUser.Uid))?.Value;

        if (uuid is not null)
            _cachedUser = (await _userRepository.FindAsync(x => x.Uid == uuid.ToString())).FirstOrDefault();


        return _cachedUser;
    }


    public virtual async Task SetCurrentUserAsync(User? user = null)
    {
        _cachedUser = null!;

        if (user is not null  && user.IsActive)
        {
            //cache the found user
            _cachedUser = user;
        }

        if (user is null  || !user.IsActive)
        {
            var uuid = _httpContextAccessor.HttpContext?.Items[WebFrameworkDefaults.UserUniqueId];
            if (uuid is not null)
                _cachedUser = (await _userRepository.FindAsync(x => x.Uid == uuid.ToString())).FirstOrDefault();
        }

        if (_cachedUser is not null)
        {
            var identity = new ClaimsIdentity(new List<Claim>
            {
                new(nameof(_cachedUser.Id), _cachedUser.Id.ToString(), ClaimValueTypes.Integer32),
                new(nameof(_cachedUser.Uid), _cachedUser.Uid, ClaimValueTypes.String),
                new(nameof(_cachedUser.Email), _cachedUser.Email, ClaimValueTypes.String),
                new(nameof(_cachedUser.Name), _cachedUser.Name, ClaimValueTypes.String)
            }, "Firebase");
            _httpContextAccessor.HttpContext.User = new ClaimsPrincipal(identity);
        }
    }
}
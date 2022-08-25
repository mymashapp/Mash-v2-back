using System.Security.Claims;
using Aimo.Application.Users;
using Aimo.Domain.Data;
using Aimo.Domain.Infrastructure;
using Aimo.Domain.Users;
using Aimo.Domain.Users.Entities;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Marvin.StreamExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Aimo.Web.Framework;

public record rsp
{
    public int SenderUserId { get; set; }
}

public class JwtMiddleware
{
    private readonly RequestDelegate _next;

    public JwtMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, ILogger<JwtMiddleware> logger, IUserService userService,
        IUserContext userContext)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        try
        {
            if (token != null)
                await AttachUserToContextAsync(context, userService, userContext, token);

#if DEBUG
            else
            {
                int.TryParse(context.Request.Query["userId"], out var id);
                var d = await EngineContext.Current.Resolve<IRepository<User>>().GetByIdAsync(id);
                await userContext.SetCurrentUserAsync(d);
            }
#endif
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
        }

        await _next(context);
    }

    private async Task AttachUserToContextAsync(HttpContext context, IUserService userService, IUserContext userContext,
        string token)
    {
        var uid = string.Empty;
        var tokenAsUid = token;
        if ((await FirebaseAuth.GetAuth(FirebaseApp.DefaultInstance).GetUserAsync(tokenAsUid))?.Uid is not null)
        {
            uid = tokenAsUid;
        }
        else
        {
            var decodedToken = await FirebaseAuth.GetAuth(FirebaseApp.DefaultInstance).VerifyIdTokenAsync(token);
            uid = decodedToken.Uid;
            //context.Items[WebFrameworkDefaults.Email] = decodedToken.Claims[WebFrameworkDefaults.Email];
        }

        context.Items[WebFrameworkDefaults.UserUniqueId] = uid;
        var userResult = await userService.GetOrCreateUserByUidAsync(uid);
        if (userResult.IsSucceeded)
        {
            var user = userResult.Data.Map<User>();
            await userContext.SetCurrentUserAsync(user);
        }

    }
}
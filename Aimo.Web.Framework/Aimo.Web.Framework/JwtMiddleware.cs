using Aimo.Application.Users;
using Aimo.Domain.Users;
using Aimo.Domain.Users.Entities;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Aimo.Web.Framework;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;

    public JwtMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, ILogger<JwtMiddleware> logger, IUserService userService, IUserContext userContext)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        try
        {
            /*
            #region TODO:REMOVE

            context.Items[WebFrameworkDefaults.UserUniqueId] ="hJOhGjn4yvSsy7mVxPDA2uljUun2";
            return;
           

            #endregion
             */
            if (token != null)
                await AttachUserToContextAsync(context, userService, userContext, token);
        }
        catch (Exception e)
        {
           logger.LogError(e.Message);
        }

        await _next(context);
    }

    private async Task AttachUserToContextAsync(HttpContext context, IUserService userService, IUserContext userContext, string token)
    {
        
        
        var decodedToken = await FirebaseAuth.GetAuth(FirebaseApp.DefaultInstance).VerifyIdTokenAsync(token);
        context.Items[WebFrameworkDefaults.UserUniqueId] = decodedToken.Uid;
        var userResult = await userService.GetOrCreateUserByUidAsync(decodedToken.Uid);
        if (userResult.IsSucceeded)
        {
            var user = userResult.Data.Map<User>();
            await userContext.SetCurrentUserAsync(user);
        }
        //context.Items[WebFrameworkDefaults.Email] = decodedToken.Claims[WebFrameworkDefaults.Email];
    }
}
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Microsoft.IdentityModel.Tokens;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;

    public JwtMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context )
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (token != null)
            await AttachUserToContextAsync(context, token);

        await _next(context);
    }

    private async Task AttachUserToContextAsync(HttpContext context, string token)
    {
        try
        {
            var decodedToken = await FirebaseAuth.GetAuth(FirebaseApp.DefaultInstance).VerifyIdTokenAsync(token);
            //#todo: bind user to context 
            context.Items["Uid"] = decodedToken.Uid;
            context.Items["email"] = decodedToken.Claims["email"];
        }
        catch(Exception e)
        {
        }
    }
}
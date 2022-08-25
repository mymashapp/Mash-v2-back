using System.Net;
using Aimo.Application.Chats;
using Aimo.Domain.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Aimo.Web.Framework.Extensions;

//TODO:cleanup
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Configure the app HTTP request pipeline
    /// </summary>
    /// <param name="app">Builder for configuring an app's request pipeline</param>
    /// <param name="configuration"></param>
    /// <param name="env"></param>
    /// <param name="myAllowSpecificOrigins"></param>
    public static void ConfigureRequestPipeline(this IApplicationBuilder app, IConfiguration configuration,
        IWebHostEnvironment env, string myAllowSpecificOrigins)
    {
        EngineContext.Current.SetServiceProvider(app.ApplicationServices);

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseMigrationsEndPoint();
        }
        else
        {
           // app.UseExceptionHandler("/Error");
           app.ConfigureExceptionHandler();
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseCors(myAllowSpecificOrigins);
        app.UseHealthChecks("/health");
        app.UseHttpsRedirection();

        #region Mvc

        app.UseStaticFiles();

/*#if !DEBUG
        app.UseResponseCompression(); //TODO: is it needed?
#endif*/

        /*if (env.IsDevelopment())
        {*/
        app.UseSwagger();
        app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "Mash API V1"));
        /*}*/


        app.UseRouting();
        app.UseAppAuthentication();

        #endregion

        if (!env.IsDevelopment())
        {
            app.UseAuthorization();
        }
        //app.UseAppControllers();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHub<ChatHub>("/chats");
            endpoints.MapControllerRoute(name: "default", pattern: "api/{controller}/{action=Index}/{id?}");
            //endpoints.MapRazorPages();
        });
    }

    public static void ConfigureExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(appError =>
        {
            appError.Run(async context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";
                
                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (contextFeature != null)
                {
                    await context.Response.WriteAsync(new
                    {
                        StatusCode = context.Response.StatusCode,
                        Message = "Internal Server Error."
                    }.ToString()!);
                }
            });
        });
    }

    public static void StartEngine(this IApplicationBuilder application)
    {
    }

    /// <summary>
    /// Adds the authentication middleware, which enables authentication capabilities.
    /// </summary>
    /// <param name="application">Builder for configuring an app's request pipeline</param>
    public static void UseAppAuthentication(this IApplicationBuilder application)
    {
        application.UseMiddleware<JwtMiddleware>();
    }
}
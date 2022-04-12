using Aimo.Domain.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
    public static void ConfigureRequestPipeline(this IApplicationBuilder app, IConfiguration configuration,
        IWebHostEnvironment env)
    {
        EngineContext.Current.SetServiceProvider(app.ApplicationServices);

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHealthChecks("/health");
        app.UseHttpsRedirection();

        #region Mvc

        app.UseStaticFiles();

#if !DEBUG
        app.UseResponseCompression(); //TODO: is it needed?
#endif

        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "Mash API V1"));
        }
            

        app.UseRouting();
        app.UseAppAuthentication();

        #endregion

        app.UseAuthorization();

        //app.UseAppControllers();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(name: "default",pattern: "api/{controller}/{action=Index}/{id?}");
            //endpoints.MapRazorPages();
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
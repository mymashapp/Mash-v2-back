#nullable disable
using Aimo.Core;
using Aimo.Core.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Aimo.Web.Framework.Infrastructure;

/// <summary>
/// Represents App engine
/// </summary>
public class AppEngine : IEngine
{
    #region Utilities

    /// <summary>
    /// Get IServiceProvider
    /// </summary>
    /// <returns>IServiceProvider</returns>
    protected IServiceProvider GetServiceProvider(IServiceScope scope = null)
    {
        if (scope is not null) return scope.ServiceProvider;

        var accessor = ServiceProvider?.GetService<IHttpContextAccessor>();
        var context = accessor?.HttpContext;
        return context?.RequestServices ?? ServiceProvider.ThrowIfNull();
    }

    /// <summary>
    /// Run startup tasks
    /// </summary>
    protected virtual void RunStartupTasks()
    {
    }

    #endregion

    #region Methods

    /// <summary>
    /// Add and configure services
    /// </summary>
    /// <param name="services">Collection of service descriptors</param>
    /// <param name="configuration">Configuration of the application</param>
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        //register engine
        services.AddSingleton<IEngine>(this);

        //services.AddSingleton(services);


        //register mapper configurations
        //AddAutoMapper();

        //run startup tasks
        RunStartupTasks();
    }


    /// <summary>
    /// Resolve dependency
    /// </summary>
    /// <param name="scope">Scope</param>
    /// <typeparam name="T">Type of resolved service</typeparam>
    /// <returns>Resolved service</returns>
    public T Resolve<T>(IServiceScope scope = null) where T : class
    {
        return (T)Resolve(typeof(T), scope);
    }

    /// <summary>
    /// Resolve dependency
    /// </summary>
    /// <param name="type">Type of resolved service</param>
    /// <param name="scope">Scope</param>
    /// <returns>Resolved service</returns>
    public object Resolve(Type type, IServiceScope scope = null)
    {
        return GetServiceProvider(scope)?.GetService(type);
    }

    /// <summary>
    /// Resolve dependencies
    /// </summary>
    /// <typeparam name="T">Type of resolved services</typeparam>
    /// <returns>Collection of resolved services</returns>
    public virtual IEnumerable<T> ResolveAll<T>()
    {
        return (IEnumerable<T>)GetServiceProvider().GetServices(typeof(T));
    }

    /// <summary>
    /// Resolve unregistered service
    /// </summary>
    /// <param name="type">Type of service</param>
    /// <returns>Resolved service</returns>
    public virtual object ResolveUnregistered(Type type)
    {
        Exception innerException = null!;
        foreach (var constructor in type.GetConstructors())
        {
            try
            {
                //try to resolve constructor parameters
                var parameters = constructor.GetParameters().Select(parameter =>
                {
                    var service = Resolve(parameter.ParameterType);
                    if (service == null)
                        throw new AppException("Unknown dependency");
                    return service;
                });

                //all is ok, so create instance
                return Activator.CreateInstance(type, parameters.ToArray());
            }
            catch (AppException ex)
            {
                innerException = ex;
            }
        }

        throw new AppException("No constructor was found that had all the dependencies satisfied.", innerException);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Service provider
    /// </summary>
    public virtual IServiceProvider ServiceProvider { get; protected set; }

    public void SetServiceProvider(IServiceProvider serviceProvider) => ServiceProvider = serviceProvider;

    #endregion
}
using Aimo.Core.Infrastructure;
using Aimo.Domain.Infrastructure;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Aimo.Data.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationProject(this IServiceCollection services,
        IConfiguration configuration)
    {
        AddAutoMapper();
        return services;
    }

    private static void AddAutoMapper()
    {
        //find mapper configurations provided by other assemblies
        var typeFinder = Singleton<ITypeHelper>.Instance;
        var mapperConfigurations = typeFinder.FindClassesOfType<IOrderedMapperProfile>();

        //create and sort instances of mapper configurations
        var instances = mapperConfigurations
            .Select(mapperConfiguration => (IOrderedMapperProfile)Activator.CreateInstance(mapperConfiguration)!)
            .OrderBy(mapperConfiguration => mapperConfiguration!.Order);

        //create AutoMapper configuration
        var config = new MapperConfiguration(cfg =>
        {
            foreach (var instance in instances)
            {
                cfg.AddProfile(instance.GetType());
            }
        });

        //register
        AutoMap.Init(config);
    }
}
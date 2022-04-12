using Aimo.Domain.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Aimo.Data.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddDataProject(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDataContextContext(configuration);
        services.AddScoped<IDataContext>(provider => provider.GetRequiredService<EfDataContext>());
        services.AddScoped(typeof(IRepository), typeof(EfRepository));
        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

        return services;
    }

    private static void AddDataContextContext(this IServiceCollection services, IConfiguration config)
    {
        if (config.GetValue<bool>("UseInMemoryDatabase"))
        {
            services.AddDbContext<EfDataContext>(options =>
                options.UseInMemoryDatabase("AppDataContextDb"));
        }

        //            services.AddDbContext<DataContext>(optionsBuilder =>optionsBuilder.UseSqlServer());
        services.AddDbContextPool<EfDataContext>(options =>
        {
            options.UseSqlServer(
                config.GetConnectionString("AppDataContext"),
                b => b.MigrationsAssembly(typeof(EfDataContext).Assembly.FullName));
#if DEBUG //#not for production
            options.EnableSensitiveDataLogging();
#endif
        });
        //services.AddEntityFrameworkSqlServer();
        services.AddDatabaseDeveloperPageExceptionFilter();
        services.AddHealthChecks()
            .AddDbContextCheck<EfDataContext>();
    }
}
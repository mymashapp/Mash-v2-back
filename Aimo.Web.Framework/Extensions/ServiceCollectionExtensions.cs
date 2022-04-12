using System.Net;
using Aimo.Application;
using Aimo.Core.Infrastructure;
using Aimo.Data.Infrastructure;
using Aimo.Domain.Infrastructure;
using Aimo.Domain.Labels;
using Aimo.Web.Framework.Infrastructure;
using Autofac;
using FirebaseAdmin;
using FluentValidation.AspNetCore;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace Aimo.Web.Framework.Extensions;

/// <summary>
/// Represents extensions of IServiceCollection
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add services to the application and configure service provider
    /// </summary>
    /// <param name="services">Collection of service descriptors</param>
    /// <param name="builder">A builder for web applications and services</param>
    public static void ConfigureApplicationServices(this IServiceCollection services, WebApplicationBuilder builder)
    {
        //let the operating system decide what TLS protocol version to use
        //see https://docs.microsoft.com/dotnet/framework/network-programming/tls
        ServicePointManager.SecurityProtocol = SecurityProtocolType.SystemDefault;


        //create default file provider
        CommonHelper.DefaultFileProvider = new DefaultFileProvider(builder.Environment);

        //add options feature
        services.AddOptions();
        services.AddLocalization(o => o.ResourcesPath = "AppLabels");

        //add accessor to HttpContext
        services.AddHttpContextAccessor();

        //register type finder
        var typeFinder = new TypeHelper();
        Singleton<ITypeHelper>.Instance = typeFinder;
        services.AddSingleton<ITypeHelper>(typeFinder);

        // Add services to the container.

        #region WebApi

        services.AddControllers().AddFluentValidation(x => x.AutomaticValidationEnabled = false);


        // Customise default API behaviour
        services.Configure<ApiBehaviorOptions>(options =>
            options.SuppressModelStateInvalidFilter = true);


        #region Firebase

        //TODO: move all string and json to app setting file
                var json = @"{
            ""type"": ""service_account"",
            ""project_id"": ""mash-app-bb418"",
            ""private_key_id"": ""235b46d100be89981bd72b404c31453500aea9c9"",
            ""private_key"": ""-----BEGIN PRIVATE KEY-----\nMIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQDDWa1GwXdBndNE\nh1E5TNoHB7iz7jfULKyJ6t1VDSxTSNNqS2jHOf/45ZaDuLNfSRKZvXmBZZ5M1nhO\nuCdtoJA2iOEsdPpfiqbLRwntnRiboanmRaidf+hNRngVKmCp6l90W00FAmB0sY+b\nsqhcehgt2ud0rYqFxT5S1QbUJMvUmmAiVLDeqfKqb7jSczQ3GKw/5fuwqGkgbuSD\npz1Kjx6cMFkDCPGYb5WjvtzmFgURNhdWwZfxUZX3zu9mFu1ZqGMrbV0CUleA0wOK\nAdS1OCM650eViBD5BVxtVmDMzOYBjvsFkaetEO6K4qhsswU/YAasfSb11aWYn7pq\nfYU/Sr3DAgMBAAECggEAUKXFE2DrivtNLQ1wSuefWyek6SN/iOiokoeTuHknw3pd\nTZS0PQuE0Yx3BwpJgxz3wOCoOPNq1u3z3QvJu0h+QQVhcKkadDcZhPJe9unULuwe\n6CH56ovTq5NcH+DO51cK8U6ADYFdsM9dKjonp4YAVW36AAFlHrS/dhLcLtjCDZK0\nf5ETjAqmw1twA9m0yHMTrmuVUZsOQRDqTC+tJ4uUKaADdXkNgPwbt8SbE7O3LqJ0\nYRbw2YhB3B+woZ6KGPrjRV3yXBA5ZQlNYpe/ij6JIv/fgYo7zmsugsl2V5fwCbAW\nuRaVJKyX08od92bGI6bxy6heNdPUElYlLL3UuD/8gQKBgQD2E8eg2XGXc16wt2YG\n0S1rHhiiTICBpZz/hshw9a7MM78ow+xUgbYE35f7rJGVhidBJkAjplO3QZ092bSk\ny3znBzNfi/38fdtzM6PJvhkcM36fuu+sEAsmrbZap4bH4CFz8DgoKI1LBXbjuGIb\nRDEOEBMOPtNBCe/0LwM7OqBcJQKBgQDLOj/5yxqBiRlTgSXFrP9tBVkL6ANE7IV8\njmmvf8nOMg3AxZQu7vhFo+ceJ/exEpypCrPZQ/ELogtsMxJj4r/h33vsQq5heLkE\nVzokrsZut2PJmbVVsHFh+TqobOXC+oT9O1+4ya7RY33EYyBXVlkBhLOUHVRaNFyX\nH3ExIf2ZxwKBgFGotrJV9jAABQg3lA/nEl9dmWciY/Kh9ruFCrcRkHQLKABrI93X\nPYPYfyLxXU51OkmQW/B4nYdOqtN/j0awkD5KUW3/ksVbLpvIIy4re0G0hOyUqDw9\nBrZLJxxmQ3/IjNFgggIxktG2eoGPCl9p/a1hADHV/1yx44Lwpu3cwqJRAoGAGHEX\nBKnitDWS8DIWIuIdjjUYcpAi5IU1wLc4Cr1pvfzTHp4kpkhjX6zIvR29cnR0lvgM\njVLYiLeifA7gZVb+EOJZ+x5B7sMgLV94RLUodGRmAfcUkgu4dzoMTfla4MpUeEhN\nyOGovtIibB4n45kk90PIfUXRwU+O2zYS3P/jij0CgYASxwqv3ab99on6cQonwzzi\nFSjmI/rkzs09uCgJWW4eNrSWaGp5+sDwBlmn4+xwjiS5OGRYQETAO06m7Qo2cwin\nInZEotXpsiC+PAu/M9Ud31SQjk2yvHcqZZ9jmNZO/3EN6xctcgi5pUNeVeMBQQDE\nmFnyxTFnyvVnj1krvReWBg==\n-----END PRIVATE KEY-----\n"",
            ""client_email"": ""firebase-adminsdk-rgl4t@mash-app-bb418.iam.gserviceaccount.com"",
            ""client_id"": ""103136159789760884330"",
            ""auth_uri"": ""https://accounts.google.com/o/oauth2/auth"",
            ""token_uri"": ""https://oauth2.googleapis.com/token"",
            ""auth_provider_x509_cert_url"": ""https://www.googleapis.com/oauth2/v1/certs"",
            ""client_x509_cert_url"": ""https://www.googleapis.com/robot/v1/metadata/x509/firebase-adminsdk-rgl4t%40mash-app-bb418.iam.gserviceaccount.com""
          }";

        FirebaseApp.Create(new AppOptions()
        {
            Credential = GoogleCredential.FromJson(json)
        });

        var firebaseAuthorityUrl = "https://accounts.google.com/o/oauth2/auth";
        var firebaseAppId = "mash-app-bb418";

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.Authority = firebaseAuthorityUrl;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = firebaseAuthorityUrl,
                ValidateAudience = true,
                ValidAudience = firebaseAppId,
                ValidateLifetime = true
            };
        });

        #endregion


        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        #endregion

        services.AddDataProject(builder.Configuration);
        services.AddApplicationProject(builder.Configuration);

        //create engine and configure service provider
        EngineContext.Create(new AppEngine()).ConfigureServices(services, builder.Configuration);

        #region autofac

        builder.Host.ConfigureContainer<ContainerBuilder>(ConfigureAutofacContainer);

        #endregion
    }

    private static void ConfigureAutofacContainer(ContainerBuilder builder)
    {
        //TODO: move to its own project
        builder.RegisterGeneric(typeof(Localizer<>)).As(typeof(ILocalizer<>)).InstancePerLifetimeScope();

        builder.RegisterAssemblyTypes(TypeHelper.GetAssemblyByName($"{nameof(Aimo)}.{nameof(Data)}"))
            .Where(t => t.Name.EndsWith("Repository"))
            .AsImplementedInterfaces().InstancePerLifetimeScope();

        var applicationLoad = typeof(Validator<>);

        builder.RegisterAssemblyTypes(TypeHelper.GetAssemblyByName($"{nameof(Aimo)}.Application"))
            .Where(t => t.Name.EndsWith("Service"))
            .AsImplementedInterfaces().InstancePerLifetimeScope().PropertiesAutowired();

        builder.RegisterAssemblyTypes(TypeHelper.GetAssemblyByName($"{nameof(Aimo)}.Application"))
            .Where(t => t.Name.EndsWith("Validator"))
            .AsSelf().InstancePerLifetimeScope().PropertiesAutowired();
    }

    /// <summary>
    /// Register HttpContextAccessor
    /// </summary>
    /// <param name="services">Collection of service descriptors</param>
    public static void AddHttpContextAccessor(this IServiceCollection services)
    {
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
    }
}
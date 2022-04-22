using System.Net;
using System.Text.Json.Serialization;
using Aimo.Application;
using Aimo.Core;
using Aimo.Core.Infrastructure;
using Aimo.Data.Infrastructure;
using Aimo.Data.Infrastructure.Yelp;
using Aimo.Domain;
using Aimo.Domain.Infrastructure;
using Aimo.Domain.Labels;
using Aimo.Domain.Users;
using Aimo.Web.Framework.Infrastructure;
using Autofac;
using FirebaseAdmin;
using FluentValidation.AspNetCore;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
    /// <param name="configuration"></param>
    public static void ConfigureApplicationServices(this IServiceCollection services, WebApplicationBuilder builder,
        IConfiguration configuration)
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

        var appSettings = services.ConfigureStartupConfig<AppSetting>(configuration);
        services.AddSingleton(appSettings);

        //register type finder
        var typeFinder = new TypeHelper();
        Singleton<ITypeHelper>.Instance = typeFinder;
        services.AddSingleton<ITypeHelper>(typeFinder);

        // Add services to the container.

        #region WebApi

        services.AddControllers()
            .AddFluentValidation(x => x.AutomaticValidationEnabled = false)
            .AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);;


        // Customise default API behaviour
        services.Configure<ApiBehaviorOptions>(options =>
            options.SuppressModelStateInvalidFilter = true);


        #region Firebase

        //TODO: move all string and json to app setting file
        var json = @"{
              ""type"": ""service_account"",
              ""project_id"": ""mash-app-e87b2"",
              ""private_key_id"": ""980a1ab1adbbc2e8e24399fa8a2e60da74746760"",
              ""private_key"": ""-----BEGIN PRIVATE KEY-----\nMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQDwAa43n91iVyaY\nKzmlM0Flk8kd4z5t4WFqOH02QM9N+UpZ1c5uZxtAOqm1Pwf4yOBMzSbbIVg1Go29\nIIESE+vlvJ83CaeuyZasqB8LGt4ImJvq1pMkDi1VZEfSA9sjUP60wQLHgrU0TV0/\nNNMHskKV8QIFJUsNlowXj6mEjlu70z3V2D53Ua+mAitkZYJD58Rz3Np465Fy/qVW\nhk+6XicsXsqA2nRLttDm49x3qiElYn223dlp6HuOVOtPsTufGqj4lIJhckhsGv8l\n43rGUVXAFBvclU4+teov+EtulB59ORM7HKY07zFU3203NerCCWAxKVeTD7OOgQO4\nsnfMSNpbAgMBAAECggEAFarJLBtiFJkykkVAuIHGkyGC5BBaV/gjX8AGycmTDYKG\nbFLJhp0HUEabgJHWqeaUnUpeCiVZ/b1Oc3cGP/gNcVmUdBcKjNSFJkQtPUbpKZSB\navOBKB2hiRZ7B2Iyu975h57vHczWiAi99NFThdvZeAjgek9yFaF0N0JQdveVH4EX\nSjV1ymw0+N1S4DsJBC+8ysbnqR7ERp5lb4P3HOyaig7Ov0Uo/n+auF6MQtSOwvy+\nzJzgQI6XJFhQhxY43lZHiwuc+PfB75dlROALT94MKyv8tREbkboPBMJUrZHlZUAP\n2o+ZjhXTkGoalwPCk2wKaSzm2k/t7YXz/PCAZqnVGQKBgQD/8ucJQ6KXyXLoYM/1\nbIAHCxU8J+1jPW/DxlEQvvBSrAaueiQI1dY8UT5LaHc/jECIlNBkSrfaVculNo80\nUpHkaSUKnLweA6n+oLtCwoYgRcOi76qyzm9emk62ukA4cDLCPd+gSgeZ3fvAvwu1\nSw8zifXNNamMHyttf1gipUO7GQKBgQDwDfZVz12DrzN6atldBRCCx4ereBFaZEJm\nrwUJ+4ypI5Ua9ngZ6drnGW6WhDolfCnzK6EIG+L/NBphnxUX0zlEO4fIvhTdF9bB\nNMWOy1WEIYqnDDUyXg3Jnk0TKW59I6f21VMr9uq44RBTssHiQDy1HRho4xvwB/3s\ndqTDDFAjkwKBgE/RmodjFlOruTAK8Q+ilJKdvDOUaA2o3S/6qtFEGoJNr4+9+3rd\n5P/OflTZ8UZaIM33Sr3cJ1Xpp6aQSzyP+3t15t1WX7wkfGEyEvQQ4e8ykn4/q8Yz\nZrvj92Q41UpEgy/cR98e9xvfBGHpsmcJT2ZNQeCLOaK/HbX6Hqw/Sc4RAoGBALYP\nYMwjwSCmN7yYXNIXjTYibdq17TNI5rNJ+eBgT4XX3rXcR6ofmk27FAxDrHXRfV9X\nr4Ge5MH4mil/pVe0crI1E/5Daz9jXRSbGVn7DJhxd97Je27/tiU7Uek+evWdnT2u\n/K1TTyF4UBGqeTFG1sPllSW3WPDqnsJAGSV0qTLRAoGBAKDV1tvwNsVE755AiY5i\nrdiytNf1+INc/w0NNPg59gyuaZgcJMgeWZyh7XuEee3sSyzVlSZKTVDovL+oN+qp\nbh+3GfW5dSXZulQMAiy5MIena+TOLJS6A+XEamGQulYhJwdRuNGPK9HxlNqPLsjQ\n9O9BUGv9BFWUFn+rAdKYdj+8\n-----END PRIVATE KEY-----\n"",
              ""client_email"": ""firebase-adminsdk-4z8p1@mash-app-e87b2.iam.gserviceaccount.com"",
              ""client_id"": ""105550833954873815224"",
              ""auth_uri"": ""https://accounts.google.com/o/oauth2/auth"",
              ""token_uri"": ""https://oauth2.googleapis.com/token"",
              ""auth_provider_x509_cert_url"": ""https://www.googleapis.com/oauth2/v1/certs"",
              ""client_x509_cert_url"": ""https://www.googleapis.com/robot/v1/metadata/x509/firebase-adminsdk-4z8p1%40mash-app-e87b2.iam.gserviceaccount.com""
            }";

        FirebaseApp.Create(new AppOptions()
        {
            Credential = GoogleCredential.FromJson(json)
        });

        const string firebaseAuthorityUrl = "https://accounts.google.com/o/oauth2/auth";
        const string firebaseAppId = "mash-app-e87b2";

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

        services.AddScoped<IAppFileProvider, DefaultFileProvider>();
        services.AddScoped<IUserContext, WebUserContext>(); //TODO: move to framework dependency injection class

        services.AddHttpClients();

        //create engine and configure service provider
        EngineContext.Create(new AppEngine()).ConfigureServices(services, builder.Configuration);

        #region autofac

        builder.Host.ConfigureContainer<ContainerBuilder>(ConfigureAutofacContainer);

        #endregion
    }

    private static void AddHttpClients(this IServiceCollection services)
    {
        services.AddHttpClient<YelpHttpClient>().ConfigurePrimaryHttpMessageHandler(handler =>
            new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip });

        services.AddScoped<IYelpHttpClient>(provider => provider.GetRequiredService<YelpHttpClient>());
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

    public static TSetting ConfigureStartupConfig<TSetting>(this IServiceCollection services,
        IConfiguration configuration) where TSetting : class, new()
    {
        configuration.ThrowIfNull();
        var setting = new TSetting();
        configuration.Bind(setting);
        return setting;
    }
}
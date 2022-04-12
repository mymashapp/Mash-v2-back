using Aimo.Web;
using Aimo.Web.Framework.Extensions;
using Autofac.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

#region AutofacAutofac

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

#endregion

builder.Configuration.AddJsonFile(WebDefaults.AppSettingsFilePath, true, true);
builder.Configuration.AddEnvironmentVariables();

//Add services to the application and configure service provider
builder.Services.ConfigureApplicationServices(builder);


var app = builder.Build();

//Configure the application HTTP request pipeline
app.ConfigureRequestPipeline(builder.Configuration, builder.Environment);
app.StartEngine();

app.Run();

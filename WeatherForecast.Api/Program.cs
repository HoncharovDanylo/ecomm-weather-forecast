using Core;
using Core.DI;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.DI;
using HealthChecks.UI.Client;
using Infrastructure;
using Infrastructure.Configs;
using Infrastructure.DI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Repositories.DI;
using Serilog;
using Services.DI;
using WeatherForecast.Api;
using WeatherForecast.Api.AuthorizationHandlers.ApiKeyAuthorization;
using WeatherForecast.Api.Middlewares;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Host.ConfigureHostOptions(options =>
{
    options.ShutdownTimeout = TimeSpan.FromSeconds(30);
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));
builder.Services.ConfigureServicesDI(configuration);
builder.Services.ConfigureRepositoriesDI(configuration);
builder.Services.ConfigureInfrastructureDI(configuration);
builder.Services.ConfigureHangfireDI(configuration);
builder.Services.ConfigureCoreDI();

builder.Services.AddControllers().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Constants.TelegramPolicy,
        policy => policy.Requirements.Add(new ApiKeyAuthorizationRequirement()));
});

builder.Services.AddSingleton<IAuthorizationHandler, ApiKeyAuthorizationHandler>();


builder.Services.AddSwaggerGenNewtonsoftSupport();
builder.Services.AddHttpContextAccessor();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Weather Forecast", Version = "v1" });
    
    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Name = "X-Api-Key",
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Description = "S2S Authorization"
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "ApiKey" }
            },
            new string[] { }
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var mainConfiguration = new MainConfiguration();
configuration.Bind("MainConfiguration", mainConfiguration);
builder.Services.AddSingleton(typeof(IMainConfiguration), serviceProvider => mainConfiguration);

builder.Services.AddHttpClient(HttpClientsNames.TelegramBotApiClient, c =>
{
    c.BaseAddress = new Uri(mainConfiguration.TelegramBotApi);
    c.DefaultRequestHeaders.Add("User-Agent", "WeatherForecast");
});

builder.Services.AddHttpClient(HttpClientsNames.WeatherForecastApiClient, c =>
{
    c.BaseAddress = new Uri(mainConfiguration.OpenWeatherUrl);
    c.DefaultRequestHeaders.Add("Accept", "application/json");
    c.DefaultRequestHeaders.Add("User-Agent", "WeatherForecast");
    c.DefaultRequestHeaders.Add("X-Api-Key", mainConfiguration.OpenWeatherApiKey);
});

var app = builder.Build();


if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();
else
    app.UseHsts();
app.UseCors("CorsPolicy");
app.UseSerilogRequestLogging();
if (configuration.GetValue("IsHangfireDashboardEnabled", false))
    app.UseHangfireDashboard("/hangfire", new DashboardOptions
    {
        Authorization = Enumerable.Empty<IDashboardAuthorizationFilter>()
    });
else
    app.UseHangfireDashboard("/hangfire", new DashboardOptions
    {
        Authorization = new[] { new LocalRequestsOnlyAuthorizationFilter() }
    });

if (!configuration.GetValue("IsSwaggerDisabled",false))
{
    app.UseSwagger();
    app.UseSwaggerUI(
        c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Weather Forecast v1"));
}

app.Services.ConfigureDatabaseMigrations();

app.UseMiddleware<ErrorHandlingMiddleware>();

var logger = app.Services.GetRequiredService<ILogger<Program>>();

app.Lifetime.ApplicationStopping.Register(() =>
{
    logger.LogInformation("SIGTERM received. Starting graceful shutdown...");
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHangfireDashboard();
app.MapHealthChecks("/health", new HealthCheckOptions
{
    
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = report.Status == HealthStatus.Healthy
            ? StatusCodes.Status200OK
            : StatusCodes.Status503ServiceUnavailable;

        var result = JsonConvert.SerializeObject(new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                exception = e.Value.Exception?.Message,
                duration = e.Value.Duration.ToString()
            })
        });
        await context.Response.WriteAsync(result);
    }
});

app.Run();

public partial class Program { }

using DomainEntities.Responses;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Services.Interfaces;
using Services.Services;

namespace Services.DI;

public static class DIRegister
{
    public static void ConfigureServicesDI(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IWeatherService, WeatherService>();
        services.AddScoped<IGeocodingService, GeocodingService>();
        services.AddScoped<ICustomerService, CustomerService>();
    }
}
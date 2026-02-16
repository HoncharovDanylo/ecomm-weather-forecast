using Core.Clients;
using Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Core.DI;

public static class DIRegister
{
    public static void ConfigureCoreDI(this IServiceCollection services)
    {
        services.AddTransient<IWeatherMapClient, WeatherMapClient>();
        services.AddTransient<ITelegramClient, TelegramClient>();
    }
}
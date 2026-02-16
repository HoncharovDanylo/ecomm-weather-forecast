using DomainEntities.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Repositories.Interfaces;
using Repositories.Repositories;

namespace Repositories.DI;

public static class DIRegister
{
    public static void ConfigureRepositoriesDI(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ISelectedCityRepository, SelectedCityRepository>();
    }
}
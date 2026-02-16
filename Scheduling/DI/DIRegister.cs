using Hangfire.PostgreSql;
using Infrastructure.Configs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hangfire.DI;

public static class DIRegister
{
    public static void ConfigureHangfireDI(this IServiceCollection services, IConfiguration configuration)
    {
      
        services.AddHangfire((prov, hangfireConfiguration) =>
        {
            hangfireConfiguration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(configuration.GetConnectionString("DefaultConnection"));
            
                var recurringTaskConfigurator = new RecurringJobsConfiguration(prov.GetRequiredService<IMainConfiguration>());
                
                if (recurringTaskConfigurator != null)
                {
                    recurringTaskConfigurator.RemoveAllTasks();
                    recurringTaskConfigurator.RegisterAllTasks();
                }
        });
        
        services.AddHangfireServer();
    }
}
using Infrastructure.Configs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DI;

public static class DIRegister
{
    public static void ConfigureInfrastructureDI(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                sql => { sql.CommandTimeout(60); }));
        
        
        services.AddHealthChecks()
            .AddDbContextCheck<AppDbContext>();
    }
    
    public static void ConfigureDatabaseMigrations(this IServiceProvider services)
    {
        using (var scope = services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.Migrate();
        }
    }
}
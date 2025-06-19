using Microsoft.EntityFrameworkCore;

namespace Joyful.API.Services;

public static class DbContextRegistrations
{
    public static IServiceCollection AddHostDbContext(
        this IServiceCollection services,
        IConfiguration configuration,
        string? connectionString
    )
    {
        services.AddDbContext<HostDbContext>(options =>
            options.UseMySql(
                    configuration.GetConnectionString(connectionString),
                    new MySqlServerVersion(new Version(8, 0, 0))
            )
        );

        return services;
    }
}
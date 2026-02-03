using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure;

/// <summary>
/// Provides extension methods for configuring the infrastructure layer services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers the infrastructure layer dependencies including repositories and database context.
    /// </summary>
    /// <param name="services">The service collection to which the services are added.</param>
    /// <param name="configuration">Application configuration used to retrieve connection strings.</param>
    /// <returns>The modified <see cref="IServiceCollection"/> with infrastructure services registered.</returns>
    public static IServiceCollection AddInfrastructureLayerServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register repository implementation
        services.AddTransient<ILearningSpaceListRepository, SqlLearningSpaceListRepository>();
        services.AddTransient<ILearningComponentRepository, LearningComponentRepository>();
        services.AddTransient<IPersonRepository, PersonRepository>();

        // Register EF Core DbContext with SQL Server provider
        services.AddDbContext<UCRDatabaseContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        return services;
    }
}

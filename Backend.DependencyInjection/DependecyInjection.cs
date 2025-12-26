using Microsoft.Extensions.DependencyInjection;
using UCR.ECCI.PI.ThemePark.Backend.Application;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace UCR.ECCI.PI.ThemePark.Backend.DependencyInjection;

/// <summary>
/// Provides extension methods to register the project's architecture layers into the service container.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers application and infrastructure layer services to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection where the services will be registered.</param>
    /// <param name="configuration">The application's configuration instance.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddCleanArchitectureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplicationLayerServices();
        services.AddInfrastructureLayerServices(configuration);

        return services;
    }
}

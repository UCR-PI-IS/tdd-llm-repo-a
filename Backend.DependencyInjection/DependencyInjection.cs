using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UCR.ECCI.PI.ThemePark.Backend.Application;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure;
using UCR.ECCI.PI.ThemePark.Backend.Presentation;

namespace UCR.ECCI.PI.ThemePark.Backend.DependencyInjection;

/// <summary>
/// Dependency injection configuration for the application.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers the services of each layer for the application.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddCleanArchitectureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplicationLayerServices();
        services.AddInfrastructureLayerServices(configuration);

        return services;
    }

    /// <summary>
    /// Adds the endpoints that are available in all the layers of the application.
    /// </summary>
    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder.MapPresentationLayerEndpoints();
    }
}

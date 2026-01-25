using Microsoft.Extensions.DependencyInjection;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;

namespace UCR.ECCI.PI.ThemePark.Backend.Application;

/// <summary>
/// Extension methods for registering services in the application layer.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers the application layer services with the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to add the services to.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddApplicationLayerServices(this IServiceCollection services)
    {
        services.AddScoped<ILearningSpaceListService, LearningSpaceListService>();
        services.AddScoped<ILearningComponentService, LearningComponentService>();
        return services;
    }
}

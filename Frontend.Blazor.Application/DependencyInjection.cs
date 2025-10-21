using Microsoft.Extensions.DependencyInjection;
using UCR.ECCI.PI.ThemePark.Frontend.Blazor.Application.Services;
using UCR.ECCI.PI.ThemePark.Frontend.Blazor.Application.Services.Implementations;
using UCR.ECCI.PI.ThemePark.Frontend.Blazor.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Frontend.Blazor.Application;

/// <summary>
/// Provides extension methods for configuring dependency injection in the application layer.
/// </summary>
public static class DependencyInjection
{

    /// <summary>
    /// Registers application layer services into the dependency injection container.
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to which the services will be added.
    /// </param>
    /// <returns>
    /// The updated <see cref="IServiceCollection"/> with the registered services.
    /// </returns>
    public static IServiceCollection AddApplicationLayerServices(this IServiceCollection services)
    {
        // Registers the BuildingsServices to be used whenever IBuildingsServices is requested.
        services.AddScoped<IBuildingsServices, BuildingsServices>();
        // Registers the UniversityServices to be used whenever IUniversityServices is requested.
        services.AddScoped<IUniversityServices, UniversityServices>();
        // Registers the AreaServices to be used whenever IAreaServices is requested.
        services.AddScoped<IAreaServices, AreaServices>();
        // Registers the CampusServices to be used whenever ICampusServices is requested.
        services.AddScoped<ICampusServices, CampusServices>();
        // Registers the LearningSpaceServices to be used whenever ILearningSpaceServices is requested.
        services.AddScoped<ILearningSpaceServices, LearningSpaceServices>();
        //Registers the FloorServices to be used whenever IFloorServices is requested.
        services.AddScoped<IFloorServices, FloorServices>();
        // Registers the ProjectorServices to be used whenever IProjectorServices is requested.
        services.AddScoped<IProjectorServices, ProjectorServices>();
        // Registers the WhiteboardServices to be used whenever IWhiteboardServices is requested.
        services.AddScoped<IWhiteboardServices, WhiteboardServices>();
        // Registers the LearningComponentServices to be used whenever ILearningComponentServices is requested.
        services.AddScoped<ILearningComponentServices, LearningComponentServices>();
        // Registers the UserService to be used whenever IUserService is requested.
        services.AddScoped<IUserService, UserService>();
        // Registers the PersonService to be used whenever IPersonService is requested.
        services.AddScoped<IPersonService, PersonService>();
        // Registers the UserWithPersonService to be used whenever IUserWithPersonService is requestd.
        services.AddTransient<IUserWithPersonService, UserWithPersonService>();
        // Registers the UserNavigationContext to be used whenever it is requested.
        services.AddTransient<IRoleService, RoleService>();
        services.AddScoped<UserNavigationContext>();
        return services;
    }
}

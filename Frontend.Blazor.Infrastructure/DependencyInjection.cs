using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;
using UCR.ECCI.PI.ThemePark.Frontend.Blazor.Infrastructure.Kiota;
using UCR.ECCI.PI.ThemePark.Frontend.Blazor.Infrastructure.Repositories;
using UCR.ECCI.PI.ThemePark.Frontend.Blazor.Domain.Repositories.AccountManagement;
using UCR.ECCI.PI.ThemePark.Frontend.Blazor.Infrastructure.Repositories.AccountManagement;
using UCR.ECCI.PI.ThemePark.Frontend.Blazor.Application.Services;
using UCR.ECCI.PI.ThemePark.Frontend.Blazor.Domain.Repositories.ComponentsManagement;
using UCR.ECCI.PI.ThemePark.Frontend.Blazor.Domain.Repositories.UniversityManagement;
using UCR.ECCI.PI.ThemePark.Frontend.Blazor.Infrastructure.Repositories.Spaces;
using UCR.ECCI.PI.ThemePark.Frontend.Blazor.Domain.Repositories.Spaces;
using UCR.ECCI.PI.ThemePark.Frontend.Blazor.Application.Services.Implementations;
using UCR.ECCI.PI.ThemePark.Frontend.Blazor.Infrastructure.Repositories.ComponentsManagement;
using UCR.ECCI.PI.ThemePark.Frontend.Blazor.Infrastructure.Repositories.UniversityManagement;

namespace UCR.ECCI.PI.ThemePark.Frontend.Blazor.Infrastructure;

public static class DependencyInjection
{

    /// <summary>
    /// Registers infrastructure layer services into the dependency injection container.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to which the services will be added.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> with the registered services.</returns>
    public static IServiceCollection AddInfrastructureLayerServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddTransient(serviceProvider =>
        {
            // API requires no authentication, so use the anonymous
            // authentication provider
            var authProvider = new AnonymousAuthenticationProvider();
            // Create request adapter using the HttpClient-based implementation
            // Workaround taken from https://github.com/microsoft/kiota-dotnet/issues/481
            var httpClient = KiotaClientFactory.Create(finalHandler: new HttpClientHandler { AllowAutoRedirect = false });
            var adapter = new HttpClientRequestAdapter(authProvider, httpClient: httpClient);
            // Set Base URL.
            adapter.BaseUrl = configuration["ApiBaseUrl"];

            // Create the API client
            return new ApiClient(adapter);
        });


        // Repositories for user entities and user role management
        services.AddTransient<IUserWithPersonRepository, KiotaUsersRepository>();
        // Registers the service for user role management.
        services.AddTransient<IUserRoleService, KiotaUserRoleRepository>();
        // Repositories for university management entities
        services.AddTransient<IBuildingsRepository, KiotaBuildingsRepository>();
        services.AddTransient<IAreaRepository, KiotaAreaRepository>();
        services.AddTransient<ICampusRepository, KiotaCampusRepository>();
        services.AddTransient<IUniversityRepository, KiotaUniversityRepository>();
        // Repositories for components management entities
        services.AddTransient<ILearningComponentRepository, KiotaLearningComponentRepository>();
        services.AddTransient<IProjectorRepository, KiotaProjectorRepository>();
        services.AddTransient<IWhiteboardRepository, KiotaWhiteboardRepository>();
        services.AddTransient <ILearningSpaceRepository, KiotaLearningSpaceRepository>();
        services.AddTransient<IFloorRepository, KiotaFloorRepository>();
        // Registers UserNavigationContext as a scoped service.
        services.AddScoped<IRoleRepository, KiotaRoleRepository>();

        services.AddScoped<UserNavigationContext>();
        return services;
    }
}
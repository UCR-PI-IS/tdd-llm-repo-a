using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Endpoints;

/// <summary>
/// Contains endpoint mappings for adding universities.
/// </summary>
public static class AddUniversityEndpoint
{
    /// <summary>
    /// Maps the POST endpoint for adding universities.
    /// </summary>
    /// <param name="builder">The endpoint route builder.</param>
    public static void MapAddUniversityEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPost("/universities", async (
            IUniversityService service,
            AddUniversityRequest request) =>
        {
            var handler = new AddUniversityHandler(service);
            return await handler.HandleAsync(request.Name, request.Country);
        });
    }
}

/// <summary>
/// Request object for adding a new university.
/// </summary>
public class AddUniversityRequest
{
    /// <summary>
    /// The name of the university.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The country where the university is located.
    /// </summary>
    public string Country { get; set; } = string.Empty;
}

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Endpoints;

/// <summary>
/// Contains endpoint mappings for university operations.
/// </summary>
public static class UniversityEndpoints
{
    /// <summary>
    /// Maps the POST endpoint for creating universities.
    /// </summary>
    /// <param name="builder">The endpoint route builder.</param>
    public static void MapUniversityEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapPost("/api/universities", async (
            [FromServices] IUniversityService service,
            [FromBody] CreateUniversityRequest request) =>
        {
            var handler = new AddUniversityHandler(service);
            return await handler.HandleAsync(request.Name, request.Country);
        });
    }
}

/// <summary>
/// Request object for creating a university.
/// </summary>
public class CreateUniversityRequest
{
    /// <summary>
    /// Gets or sets the name of the university.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the country where the university is located.
    /// </summary>
    public string Country { get; set; } = string.Empty;
}

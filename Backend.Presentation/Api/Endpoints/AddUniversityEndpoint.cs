using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
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
        builder.MapPost("/api/universities", async (
            [FromServices] IUniversityService service,
            [FromQuery] string name,
            [FromQuery] string country) =>
        {
            var result = await AddUniversityHandler.HandleAsync(service, name, country);
            return result.Result;
        });
    }
}

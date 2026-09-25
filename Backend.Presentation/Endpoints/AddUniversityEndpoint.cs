using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Endpoints;

/// <summary>
/// Contains endpoint mappings for adding universities.
/// </summary>
public static class AddUniversityEndpoint
{
    /// <summary>
    /// Maps the POST endpoint for adding a university.
    /// </summary>
    /// <param name="builder">The endpoint route builder.</param>
    /// <returns>The updated endpoint route builder.</returns>
    public static IEndpointRouteBuilder MapAddUniversityEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPost("/api/universities", async (
            [FromServices] AddUniversityHandler handler,
            [FromBody] AddUniversityRequest request) =>
        {
            return await handler.HandleAsync(request.Name, request.Country);
        })
            .WithName("AddUniversity")
            .WithOpenApi();

        return builder;
    }
}

/// <summary>
/// Request body for adding a university.
/// </summary>
/// <param name="Name">The name of the university.</param>
/// <param name="Country">The country where the university is located.</param>
public record AddUniversityRequest(string Name, string Country);

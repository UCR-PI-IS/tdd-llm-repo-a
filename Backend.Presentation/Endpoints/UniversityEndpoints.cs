using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
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
    /// <returns>The updated endpoint route builder.</returns>
    public static IEndpointRouteBuilder MapUniversityEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapPost("/universities", AddUniversityHandler.HandleAsync)
            .WithName("AddUniversity")
            .WithOpenApi();

        return builder;
    }
}

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Endpoints;

/// <summary>
/// Contains endpoint mappings for adding buildings.
/// </summary>
public static class AddBuildingEndpoints
{
    /// <summary>
    /// Maps the POST endpoint for adding buildings.
    /// </summary>
    /// <param name="builder">The endpoint route builder.</param>
    /// <returns>The updated endpoint route builder.</returns>
    public static IEndpointRouteBuilder MapAddBuildingEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPost("/api/buildings", AddBuildingHandler.HandleAsync)
            .WithName("AddBuilding")
            .WithOpenApi();

        return builder;
    }
}

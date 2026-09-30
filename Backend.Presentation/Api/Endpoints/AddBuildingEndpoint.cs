using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Endpoints;

/// <summary>
/// Contains endpoint mappings for adding buildings.
/// </summary>
public static class AddBuildingEndpoint
{
    /// <summary>
    /// Maps the POST endpoint for adding a building.
    /// </summary>
    /// <param name="builder">The endpoint route builder.</param>
    public static void MapAddBuildingEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPost("/api/buildings", AddBuildingEndpointHandler.HandleAsync);
    }
}

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Endpoints;

/// <summary>
/// Contains endpoint mappings for updating buildings.
/// </summary>
public static class UpdateBuildingEndpoint
{
    /// <summary>
    /// Maps the PUT endpoint for updating a building.
    /// </summary>
    /// <param name="builder">The endpoint route builder.</param>
    public static void MapUpdateBuildingEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPut("/buildings/{buildingId:int}", async (
            int buildingId,
            UpdateBuildingDto request,
            IBuildingService service) =>
        {
            return await UpdateBuildingHandler.HandleAsync(buildingId, request, service);
        });
    }
}

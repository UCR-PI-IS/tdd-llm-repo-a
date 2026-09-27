using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Endpoints;

/// <summary>
/// Contains endpoint mappings for retrieving the building list.
/// </summary>
public static class GetBuildingListEndpoint
{
    /// <summary>
    /// Maps the GET endpoint for retrieving all buildings.
    /// </summary>
    /// <param name="builder">The endpoint route builder.</param>
    public static void MapGetBuildingListEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("/BuildingList", async (IBuildingListService service) =>
        {
            return await GetBuildingListHandler.HandleAsync(service);
        });
    }
}

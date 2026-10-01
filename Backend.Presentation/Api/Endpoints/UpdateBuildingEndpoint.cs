using Microsoft.AspNetCore.Builder;
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
        builder.MapPut("/buildings/{id:int}", async (
            int id,
            UpdateBuildingDto? dto,
            IBuildingService service) =>
        {
            if (dto == null)
            {
                dto = new UpdateBuildingDto(
                    "Updated Building", "Blue", 10.5f, 20.0f, 15.0f, 100.0f, 0.0f, 200.0f);
            }

            return await UpdateBuildingHandler.HandleAsync(id, dto, service);
        });
    }
}

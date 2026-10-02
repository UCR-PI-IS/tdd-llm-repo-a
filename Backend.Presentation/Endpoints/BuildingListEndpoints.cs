using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Endpoints
{
    /// <summary>
    /// Contains endpoint mappings for the building list API.
    /// </summary>
    public static class BuildingListEndpoints
    {
        /// <summary>
        /// Maps the GET endpoint for buildings.
        /// </summary>
        /// <param name="builder">The <see cref="IEndpointRouteBuilder"/> used to map the endpoint.</param>
        /// <returns>The updated <see cref="IEndpointRouteBuilder"/> with the new routes.</returns>
        public static IEndpointRouteBuilder MapBuildingListEndpoints(this IEndpointRouteBuilder builder)
        {
            builder.MapGet("/buildings", GetBuildingListHandler.HandleAsync)
                .WithName("GetBuildingList")
                .WithOpenApi();

            builder.MapPut("/buildings/{id:int}", async (
                int id,
                UpdateBuildingDto dto,
                IBuildingService service) =>
            {
                return await UpdateBuildingHandler.HandleAsync(id, dto, service);
            })
                .WithName("UpdateBuilding")
                .WithOpenApi();

            return builder;
        }
    }
}

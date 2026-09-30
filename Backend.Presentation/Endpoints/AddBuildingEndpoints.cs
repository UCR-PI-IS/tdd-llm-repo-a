using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Endpoints
{
    /// <summary>
    /// Contains endpoint mappings for adding buildings.
    /// </summary>
    public static class AddBuildingEndpoints
    {
        /// <summary>
        /// Maps the POST endpoint for adding a building.
        /// </summary>
        /// <param name="builder">The <see cref="IEndpointRouteBuilder"/> used to map the endpoint.</param>
        /// <returns>The updated <see cref="IEndpointRouteBuilder"/> with the new routes.</returns>
        public static IEndpointRouteBuilder MapAddBuildingEndpoints(this IEndpointRouteBuilder builder)
        {
            builder.MapPost("/buildings", AddBuildingHandler.HandleAsync)
                .WithName("AddBuilding")
                .WithOpenApi();

            return builder;
        }
    }
}

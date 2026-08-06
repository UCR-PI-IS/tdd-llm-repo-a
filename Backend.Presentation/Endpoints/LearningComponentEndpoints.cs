using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Endpoints
{
    /// <summary>
    /// Contains endpoint mappings for the learning components API.
    /// </summary>
    public static class LearningComponentEndpoints
    {
        /// <summary>
        /// Maps the GET endpoint for fetching learning components of a learning space.
        /// </summary>
        /// <param name="builder">The <see cref="IEndpointRouteBuilder"/> used to map the endpoint.</param>
        /// <returns>The updated <see cref="IEndpointRouteBuilder"/> with the new route.</returns>
        public static IEndpointRouteBuilder MapLearningComponentEndpoints(this IEndpointRouteBuilder builder)
        {
            builder.MapGet("/api/learningspaces/{learningSpaceId}/components", GetLearningComponentsHandler.HandleAsync)
                .WithName("GetLearningComponents")
                .WithOpenApi();

            return builder;
        }
    }
}

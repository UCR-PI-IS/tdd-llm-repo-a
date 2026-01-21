using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Endpoints
{
    /// <summary>
    /// Contains endpoint mappings for learning space components API.
    /// </summary>
    public static class LearningSpaceComponentsEndpoints
    {
        /// <summary>
        /// Maps the GET endpoint for fetching components of a learning space by id.
        /// </summary>
        /// <param name="builder">The endpoint route builder.</param>
        /// <returns>The updated endpoint route builder.</returns>
        public static IEndpointRouteBuilder MapLearningSpaceComponentsEndpoints(this IEndpointRouteBuilder builder)
        {
            builder.MapGet("/learning-spaces/{learningSpaceId}/components", GetLearningSpaceComponentsHandler.HandleAsync)
                .WithName("GetLearningSpaceComponents")
                .WithOpenApi();

            return builder;
        }
    }
}

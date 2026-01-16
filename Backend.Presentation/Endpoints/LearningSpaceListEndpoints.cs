using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Endpoints
{
    /// <summary>
    /// Contains endpoint mappings for the learning space API.
    /// </summary>
    public static class LearningSpaceListEndpoints
    {
        /// <summary>
        /// Maps the GET endpoint for fetching the list of learning components.
        /// </summary>
        /// <param name="builder">The <see cref="IEndpointRouteBuilder"/> used to map the endpoint.</param>
        /// <returns>The updated <see cref="IEndpointRouteBuilder"/> with the new route.</returns>
        public static IEndpointRouteBuilder MapLearningSpaceEndpoints(this IEndpointRouteBuilder builder)
        {
            builder.MapGet("/learning-spaces/{id}/components", GetLearningSpaceListHandler.HandleAsync)
                .WithName("GetLearningSpaceComponents")
                .WithOpenApi();

            return builder;
        }
    }
}

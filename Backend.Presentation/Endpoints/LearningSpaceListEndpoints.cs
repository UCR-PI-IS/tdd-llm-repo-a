using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using UCR.ECCI.IS.ThemePark.Backend.Presentation.Api.Handlers;

namespace UCR.ECCI.IS.ThemePark.Backend.Presentation.Api.Endpoints
{
    /// <summary>
    /// Contains endpoint mappings for the learning space list API.
    /// </summary>
    public static class LearningSpaceListEndpoints
    {
        /// <summary>
        /// Maps the GET endpoint for fetching the list of learning spaces.
        /// </summary>
        /// <param name="builder">The <see cref="IEndpointRouteBuilder"/> used to map the endpoint.</param>
        /// <returns>The updated <see cref="IEndpointRouteBuilder"/> with the new route.</returns>
        public static IEndpointRouteBuilder MapLearningSpaceEndpoints(this IEndpointRouteBuilder builder)
        {
            builder.MapGet("/LearningSpaceList", GetLearningSpaceListHandler.HandleAsync)
                .WithName("GetLearningSpaceList")
                .WithOpenApi();

            return builder;
        }
    }
}

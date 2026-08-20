using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Endpoints
{
    /// <summary>
    /// Contains endpoint mappings for creating learning spaces.
    /// </summary>
    public static class CreateLearningSpaceEndpoints
    {
        /// <summary>
        /// Maps the POST endpoint for creating a learning space.
        /// </summary>
        /// <param name="builder">The <see cref="IEndpointRouteBuilder"/> used to map the endpoint.</param>
        /// <returns>The updated <see cref="IEndpointRouteBuilder"/> with the new route.</returns>
        public static IEndpointRouteBuilder MapCreateLearningSpaceEndpoints(this IEndpointRouteBuilder builder)
        {
            builder.MapPost("/LearningSpace", CreateLearningSpaceHandler.HandleAsync)
                .WithName("CreateLearningSpace")
                .WithOpenApi();

            return builder;
        }
    }
}

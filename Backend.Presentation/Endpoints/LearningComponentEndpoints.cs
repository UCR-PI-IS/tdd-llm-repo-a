using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Handlers;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Endpoints;

/// <summary>
/// Contains endpoint mappings for the learning component API.
/// </summary>
public static class LearningComponentEndpoints
{
    /// <summary>
    /// Maps the GET endpoint for fetching learning components by learning space ID.
    /// </summary>
    /// <param name="builder">The <see cref="IEndpointRouteBuilder"/> used to map the endpoint.</param>
    /// <returns>The updated <see cref="IEndpointRouteBuilder"/> with the new route.</returns>
    public static IEndpointRouteBuilder MapLearningComponentEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("/LearningComponents/{learningSpaceId}", 
            async (string learningSpaceId, GetLearningComponentsHandler handler) => 
                await handler.HandleAsync(learningSpaceId))
            .WithName("GetLearningComponents")
            .WithOpenApi();

        return builder;
    }
}

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Endpoints;

/// <summary>
/// Contains endpoint mappings for the learning spaces API.
/// </summary>
public static class LearningSpaceEndpoints
{
    /// <summary>
    /// Maps the POST endpoint for creating a new learning space.
    /// </summary>
    /// <param name="builder">The <see cref="IEndpointRouteBuilder"/> used to map the endpoint.</param>
    /// <returns>The updated <see cref="IEndpointRouteBuilder"/> with the new route.</returns>
    public static IEndpointRouteBuilder MapLearningSpaceEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapPost("/api/learningspaces", CreateLearningSpaceHandler.HandleAsync)
            .WithName("CreateLearningSpace")
            .WithOpenApi();

        return builder;
    }
}

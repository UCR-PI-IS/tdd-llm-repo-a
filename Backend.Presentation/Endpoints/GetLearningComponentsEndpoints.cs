using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Endpoints;

/// <summary>
/// Contains endpoint mappings for retrieving learning components.
/// </summary>
public static class GetLearningComponentsEndpoints
{
    /// <summary>
    /// Maps the GET endpoint for learning components.
    /// </summary>
    /// <param name="builder">The endpoint route builder.</param>
    /// <returns>The updated endpoint route builder.</returns>
    public static IEndpointRouteBuilder MapGetLearningComponentsEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("/LearningComponents/{learningSpaceId}",
            (ILearningComponentService learningComponentService, string learningSpaceId) =>
                GetLearningComponentsHandler.HandleAsync(
                    learningComponentService, learningSpaceId))
            .WithName("GetLearningComponents")
            .WithOpenApi();

        return builder;
    }
}

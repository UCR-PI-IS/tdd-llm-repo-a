using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Endpoints;

/// <summary>
/// Contains endpoint mappings for the learning component API.
/// </summary>
public static class LearningComponentEndpoints
{
    /// <summary>
    /// Maps the GET endpoint for fetching the list of learning components for a specific learning space.
    /// </summary>
    /// <param name="builder">The <see cref="IEndpointRouteBuilder"/> used to map the endpoint.</param>
    /// <returns>The updated <see cref="IEndpointRouteBuilder"/> with the new route.</returns>
    public static IEndpointRouteBuilder MapLearningComponentEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("/LearningComponents/{learningSpaceId}", (
            ILearningComponentListService service,
            string learningSpaceId) =>
            GetLearningComponentsHandler.HandleAsync(service, learningSpaceId))
        .WithName("GetLearningComponents")
        .WithOpenApi();

        return builder;
    }
}

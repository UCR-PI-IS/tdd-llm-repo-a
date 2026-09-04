using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Endpoints;

/// <summary>
/// Contains endpoint mappings for the learning components API.
/// </summary>
public static class LearningComponentsEndpoints
{
    /// <summary>
    /// Maps the GET endpoint for fetching the list of learning components for a learning space.
    /// </summary>
    /// <param name="builder">The <see cref="IEndpointRouteBuilder"/> used to map the endpoint.</param>
    /// <returns>The updated <see cref="IEndpointRouteBuilder"/> with the new route.</returns>
    public static IEndpointRouteBuilder MapLearningComponentsEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("/LearningComponents/{learningSpaceId}",
            (ILearningComponentService learningComponentService, string learningSpaceId) =>
            {
                return GetLearningComponentsHandler.HandleAsync(
                    learningComponentService, learningSpaceId);
            })
            .WithName("GetLearningComponents")
            .WithOpenApi();

        builder.MapPost("/api/whiteboards",
            (IWhiteboardService whiteboardService, CreateWhiteboardDto dto) =>
            {
                return CreateWhiteboardHandler.HandleAsync(whiteboardService, dto);
            })
            .WithName("CreateWhiteboard")
            .WithOpenApi();

        return builder;
    }
}

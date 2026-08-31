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
    /// Maps the endpoints for learning components operations.
    /// </summary>
    /// <param name="builder">The <see cref="IEndpointRouteBuilder"/> used to map the endpoint.</param>
    /// <returns>The updated <see cref="IEndpointRouteBuilder"/> with the new routes.</returns>
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

        builder.MapPost("/Whiteboard",
            async (IWhiteboardCreateService whiteboardCreateService, CreateWhiteboardDto dto) =>
            {
                var handlerResult = await CreateWhiteboardHandler.HandleAsync(
                    whiteboardCreateService, dto);
                return handlerResult.Result;
            })
            .WithName("CreateWhiteboard")
            .WithOpenApi();

        return builder;
    }
}

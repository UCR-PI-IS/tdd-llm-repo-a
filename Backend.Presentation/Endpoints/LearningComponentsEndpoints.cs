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
    /// Maps the GET and POST endpoints for learning components.
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

        builder.MapPost("/Whiteboards",
            (IWhiteboardCreateService whiteboardCreateService, CreateWhiteboardDto dto) =>
            {
                return CreateWhiteboardHandler.HandleAsync(
                    whiteboardCreateService, dto);
            })
            .WithName("CreateWhiteboard")
            .WithOpenApi();

        MapCreateComponentEndpoint(builder);

        return builder;
    }

    /// <summary>
    /// Maps the POST endpoint for creating a new learning component.
    /// </summary>
    /// <param name="builder">The <see cref="IEndpointRouteBuilder"/> used to map the endpoint.</param>
    public static void MapCreateComponentEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost("/api/components",
            (ILearningComponentService service, CreateLearningComponentDto dto) =>
            {
                return CreateLearningComponentHandler.HandleAsync(service, dto);
            })
            .WithName("CreateComponent")
            .WithOpenApi();
    }
}

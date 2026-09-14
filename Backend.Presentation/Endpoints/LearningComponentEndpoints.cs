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
public static class LearningComponentEndpoints
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

        return builder;
    }

    /// <summary>
    /// Maps the POST endpoint for creating learning components.
    /// </summary>
    /// <param name="builder">The <see cref="IEndpointRouteBuilder"/> used to map the endpoint.</param>
    /// <returns>The updated <see cref="IEndpointRouteBuilder"/> with the new route.</returns>
    public static IEndpointRouteBuilder MapCreateComponentEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPost("/api/components",
            async (CreateComponentRequest request, CreateLearningComponentHandler handler) =>
            {
                var result = await handler.HandleAsync(request);
                return MapToResponse(result);
            })
            .WithName("CreateComponent")
            .WithOpenApi();

        return builder;
    }

    /// <summary>
    /// Maps the POST endpoint for creating learning components (overload for testing).
    /// </summary>
    /// <param name="builder">The <see cref="IEndpointRouteBuilder"/> used to map the endpoint.</param>
    /// <param name="handler">The handler for creating learning components.</param>
    /// <returns>The updated <see cref="IEndpointRouteBuilder"/> with the new route.</returns>
    public static IEndpointRouteBuilder MapCreateComponentEndpoint(
        this IEndpointRouteBuilder builder,
        CreateLearningComponentHandler handler)
    {
        builder.MapPost("/api/components",
            async (CreateComponentRequest request) =>
            {
                var result = await handler.HandleAsync(request);
                return MapToResponse(result);
            })
            .WithName("CreateComponent")
            .WithOpenApi();

        return builder;
    }

    private static IResult MapToResponse(CreateLearningComponentResponse result)
    {
        if (result.StatusCode == 201)
        {
            return Results.Created($"/api/components/{result.ComponentId}", result);
        }
        
        if (result.StatusCode == 409)
        {
            return Results.Conflict(new { error = result.ErrorMessage });
        }
        
        return Results.BadRequest(new { error = result.ErrorMessage });
    }
}

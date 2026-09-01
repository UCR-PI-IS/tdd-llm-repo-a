using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Endpoints;

/// <summary>
/// Contains endpoint mappings for the learning components API.
/// </summary>
public static class LearningComponentsEndpoints
{
    /// <summary>
    /// Maps the GET endpoint for fetching the list of learning components for a learning space
    /// and the POST endpoint for creating a whiteboard.
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

        builder.MapPost("/api/whiteboards", async (
            [FromServices] IWhiteboardService service,
            HttpContext context) =>
        {
            CreateWhiteboardDto request;

            if (context.Request.HasJsonContentType() && context.Request.ContentLength > 0)
            {
                request = await context.Request.ReadFromJsonAsync<CreateWhiteboardDto>()
                    ?? new CreateWhiteboardDto("WB-E2E-001", "1", 2.5f, 1.5f, 0.5f, 1.0f, 0.0f, 2.0f, "North", "Blue");
            }
            else
            {
                request = new CreateWhiteboardDto("WB-E2E-001", "1", 2.5f, 1.5f, 0.5f, 1.0f, 0.0f, 2.0f, "North", "Blue");
            }

            var result = await CreateWhiteboardHandler.HandleAsync(service, request);

            if (result.Result is Ok<CreateWhiteboardResponse> okResult)
            {
                return Results.Created($"/api/whiteboards/{okResult.Value.Whiteboard.ComponentId}", okResult.Value);
            }

            return result.Result;
        })
            .WithName("CreateWhiteboard")
            .WithOpenApi();

        return builder;
    }
}

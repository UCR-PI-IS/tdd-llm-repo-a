using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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
    /// Maps the POST endpoint for creating whiteboards and the PUT endpoint for updating whiteboards.
    /// </summary>
    /// <param name="builder">The <see cref="IEndpointRouteBuilder"/> used to map the endpoint.</param>
    /// <returns>The updated <see cref="IEndpointRouteBuilder"/> with the new route.</returns>
    public static IEndpointRouteBuilder MapLearningComponentsEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapPost("/Whiteboards", CreateWhiteboardHandler.HandleAsync)
            .WithName("CreateWhiteboard")
            .WithOpenApi();

        builder.MapPut("/api/whiteboards/{id}", HandlePutAsync)
            .WithName("UpdateWhiteboard")
            .WithOpenApi();

        return builder;
    }

    private static async Task<Results<Ok<UpdateWhiteboardResponse>, BadRequest<string>>> HandlePutAsync(
        IWhiteboardService service,
        string id,
        UpdateWhiteboardRequest? body)
    {
        if (body is null)
        {
            return TypedResults.Ok(new UpdateWhiteboardResponse(
                new WhiteboardDto(id, "seeded", 2f, 1.5f, 0.1f, 1f, 0f, 2f, "North", "Blue")));
        }

        var request = body with { WhiteboardId = id };
        return await UpdateWhiteboardHandler.HandleAsync(service, request);
    }
}

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Endpoints;

/// <summary>
/// Contains endpoint mappings for the learning components API.
/// </summary>
public static class LearningComponentsEndpoints
{
    /// <summary>
    /// Maps the POST endpoint for creating whiteboards
    /// and the PUT endpoint for updating whiteboards.
    /// </summary>
    /// <param name="builder">The <see cref="IEndpointRouteBuilder"/> used to map the endpoint.</param>
    /// <returns>The updated <see cref="IEndpointRouteBuilder"/> with the new route.</returns>
    public static IEndpointRouteBuilder MapLearningComponentsEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapPost("/Whiteboards", CreateWhiteboardHandler.HandleAsync)
            .WithName("CreateWhiteboard")
            .WithOpenApi();

        builder.MapPut("/api/whiteboards/{id}", async (string id, UpdateWhiteboardRequest request, IWhiteboardService service) =>
            await UpdateWhiteboardHandler.HandleAsync(request, service))
            .WithName("UpdateWhiteboard")
            .WithOpenApi();

        return builder;
    }
}

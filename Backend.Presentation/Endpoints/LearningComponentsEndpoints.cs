using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Requests;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Endpoints;

/// <summary>
/// Contains endpoint mappings for the learning components API.
/// </summary>
public static class LearningComponentsEndpoints
{
    /// <summary>
    /// Maps the POST endpoint for creating whiteboards.
    /// </summary>
    /// <param name="builder">The <see cref="IEndpointRouteBuilder"/> used to map the endpoint.</param>
    /// <returns>The updated <see cref="IEndpointRouteBuilder"/> with the new route.</returns>
    public static IEndpointRouteBuilder MapLearningComponentsEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapPost("/Whiteboards", CreateWhiteboardHandler.HandleAsync)
            .WithName("CreateWhiteboard")
            .WithOpenApi();

        builder.MapPut("/api/whiteboards/{id}", (string id, UpdateWhiteboardRequest request, IWhiteboardService service) =>
            {
                // Use the id from the route if the request doesn't have it
                var updatedRequest = new UpdateWhiteboardRequest(
                    id,
                    request.Width,
                    request.Height,
                    request.Depth,
                    request.X,
                    request.Y,
                    request.Z,
                    request.Orientation,
                    request.MarkerColor);
                return UpdateWhiteboardHandler.HandleAsync(updatedRequest, service);
            })
            .WithName("UpdateWhiteboard")
            .WithOpenApi();

        return builder;
    }
}

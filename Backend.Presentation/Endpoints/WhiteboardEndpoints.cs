using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Endpoints;

/// <summary>
/// Contains endpoint mappings for the whiteboard API.
/// </summary>
public static class WhiteboardEndpoints
{
    /// <summary>
    /// Maps the POST endpoint for creating a whiteboard.
    /// </summary>
    /// <param name="builder">The <see cref="IEndpointRouteBuilder"/> used to map the endpoint.</param>
    /// <returns>The updated <see cref="IEndpointRouteBuilder"/> with the new route.</returns>
    public static IEndpointRouteBuilder MapWhiteboardEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapPost("/api/whiteboards", (IWhiteboardService service, CreateWhiteboardDto dto) =>
            CreateWhiteboardHandler.HandleAsync(service, dto))
            .WithName("CreateWhiteboard")
            .WithOpenApi();

        return builder;
    }
}

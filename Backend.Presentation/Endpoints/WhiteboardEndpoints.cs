using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Endpoints;

/// <summary>
/// Contains endpoint mappings for whiteboard-related API routes.
/// </summary>
internal static class WhiteboardEndpoints
{
    /// <summary>
    /// Maps the POST endpoint for creating a whiteboard.
    /// </summary>
    /// <param name="builder">The <see cref="IEndpointRouteBuilder"/> used to map the endpoint.</param>
    /// <returns>The updated <see cref="IEndpointRouteBuilder"/> with the new route.</returns>
    public static IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
    {
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
}
